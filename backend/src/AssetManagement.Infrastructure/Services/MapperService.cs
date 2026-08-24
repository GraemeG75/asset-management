using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using AssetManagement.Core.Dtos;
using AssetManagement.Core.Models;
using AssetManagement.Core.Services;
using AssetManagement.Infrastructure.Data;

namespace AssetManagement.Infrastructure.Services
{
    public class MapperService : IMapperService
    {
        private readonly ISiteContext _siteContext;
        private readonly IUserContext _userContext;
        private readonly IMetadataRepository _metadataRepository;
        private readonly AppDbContext _dbContext;

        public MapperService(ISiteContext siteContext, IUserContext userContext, IMetadataRepository metadataRepository, AppDbContext dbContext)
        {
            _siteContext = siteContext;
            _userContext = userContext;
            _metadataRepository = metadataRepository;
            _dbContext = dbContext;
        }

        public async Task<Dictionary<string, object?>> LoadFormDataAsync(string formKey, string? recordId = null)
        {
            string locale = _siteContext.CurrentLocale;
            Dictionary<string, object?> formData = new Dictionary<string, object?>();

            XFormEntity? formMeta = await _metadataRepository.GetFormByKeyAsync(formKey, locale);
            if (formMeta == null)
            {
                return formData;
            }

            IEnumerable<XMapperFlavorFieldEntity>? flavorFields = null;
            if (!string.IsNullOrEmpty(formMeta.FlavorKey))
            {
                flavorFields = await _metadataRepository.GetFlavorFieldsAsync(formMeta.FlavorKey, locale);
            }

            // Retrieve current user entity directly via UserContext
            UserEntity? user = await _userContext.GetCurrentUserEntityAsync();

            if (user != null)
            {
                Type userType = typeof(UserEntity);
                if (flavorFields != null)
                {
                    foreach (XMapperFlavorFieldEntity field in flavorFields)
                    {
                        string propName = !string.IsNullOrEmpty(field.MapperFieldName) ? field.MapperFieldName : field.KeyName;
                        PropertyInfo? prop = userType.GetProperty(propName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                                          ?? userType.GetProperty(field.KeyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (prop != null)
                        {
                            formData[field.KeyName] = prop.GetValue(user);
                        }
                    }
                }
                else
                {
                    foreach (PropertyInfo prop in userType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (prop.CanRead)
                        {
                            formData[prop.Name] = prop.GetValue(user);
                        }
                    }
                }
            }

            return formData;
        }

        public async Task<List<FormFieldErrorDto>> ValidateFormDataAsync(FormSubmissionDto submission)
        {
            string locale = _siteContext.CurrentLocale;
            List<FormFieldErrorDto> errors = new List<FormFieldErrorDto>();

            if (submission == null)
            {
                errors.Add(new FormFieldErrorDto { FieldKey = "submission", Message = "Submission payload is required." });
                return errors;
            }

            XFormEntity? formMeta = await _metadataRepository.GetFormByKeyAsync(submission.FormKey, locale);
            if (formMeta != null && !string.IsNullOrEmpty(formMeta.FlavorKey))
            {
                IEnumerable<XMapperFlavorFieldEntity> flavorFields = await _metadataRepository.GetFlavorFieldsAsync(formMeta.FlavorKey, locale);
                foreach (XMapperFlavorFieldEntity field in flavorFields)
                {
                    if (field.IsReadonly || field.IsDisabled)
                    {
                        if (submission.FieldValues.TryGetValue(field.KeyName, out object? submittedVal) && submittedVal != null)
                        {
                            // Validate read-only field invariants
                        }
                    }
                }
            }

            return errors;
        }

        public async Task<FormSubmissionResultDto> SaveFormDataAsync(FormSubmissionDto submission)
        {
            string locale = _siteContext.CurrentLocale;

            if (!_userContext.IsAuthenticated || string.IsNullOrWhiteSpace(_userContext.UserId))
            {
                throw new UnauthorizedAccessException("User identity claim is missing or invalid.");
            }

            List<FormFieldErrorDto> validationErrors = await ValidateFormDataAsync(submission);
            if (validationErrors.Count > 0)
            {
                return new FormSubmissionResultDto
                {
                    Success = false,
                    Message = "Validation failed for form submission",
                    FormKey = submission.FormKey,
                    FormType = submission.FormType,
                    FieldErrors = validationErrors
                };
            }

            XFormEntity? formMeta = await _metadataRepository.GetFormByKeyAsync(submission.FormKey, locale);
            string formTitle = !string.IsNullOrWhiteSpace(formMeta?.Title)
                ? formMeta.Title
                : (!string.IsNullOrWhiteSpace(formMeta?.Caption) ? formMeta.Caption : submission.FormKey);

            IEnumerable<XMapperFlavorFieldEntity>? flavorFields = null;
            if (formMeta != null && !string.IsNullOrEmpty(formMeta.FlavorKey))
            {
                flavorFields = await _metadataRepository.GetFlavorFieldsAsync(formMeta.FlavorKey, locale);
            }

            Dictionary<string, object?> updatedData = new Dictionary<string, object?>();

            // Retrieve current authenticated person via UserContext
            UserEntity? user = await _userContext.GetCurrentUserEntityAsync();
            string primaryKey = user?.Id ?? _userContext.UserId ?? submission.RecordId ?? Guid.NewGuid().ToString();

            if (user != null)
            {
                Type userType = typeof(UserEntity);
                if (flavorFields != null)
                {
                    foreach (XMapperFlavorFieldEntity field in flavorFields)
                    {
                        if (field.IsReadonly || field.IsDisabled)
                        {
                            continue;
                        }

                        object? rawValue = null;
                        bool found = submission.FieldValues.TryGetValue(field.KeyName, out rawValue);
                        if (!found && !string.IsNullOrEmpty(field.MapperFieldName))
                        {
                            found = submission.FieldValues.TryGetValue(field.MapperFieldName, out rawValue);
                        }

                        if (rawValue != null)
                        {
                            string targetPropName = !string.IsNullOrEmpty(field.MapperFieldName) ? field.MapperFieldName : field.KeyName;
                            PropertyInfo? prop = userType.GetProperty(targetPropName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                                              ?? userType.GetProperty(field.KeyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                            if (prop != null && prop.CanWrite)
                            {
                                object? convertedValue = ConvertValue(rawValue, prop.PropertyType);
                                prop.SetValue(user, convertedValue);
                                updatedData[field.KeyName] = convertedValue;
                            }
                        }
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, object?> kvp in submission.FieldValues)
                    {
                        if (kvp.Value != null)
                        {
                            PropertyInfo? prop = userType.GetProperty(kvp.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                            if (prop != null && prop.CanWrite)
                            {
                                object? convertedValue = ConvertValue(kvp.Value, prop.PropertyType);
                                prop.SetValue(user, convertedValue);
                                updatedData[kvp.Key] = convertedValue;
                            }
                            else
                            {
                                updatedData[kvp.Key] = kvp.Value;
                            }
                        }
                    }
                }

                Guid? auditUserId = Guid.TryParse(_userContext.UserId, out Guid parsedId) ? parsedId : null;
                user.DateUpdated = DateTime.UtcNow;
                user.UpdatedById = auditUserId;

                updatedData["date_updated"] = user.DateUpdated;
                updatedData["updated_by_id"] = user.UpdatedById;

                await _dbContext.SaveChangesAsync();
            }

            return new FormSubmissionResultDto
            {
                Success = true,
                Message = $"Form '{formTitle}' saved successfully",
                RecordId = primaryKey,
                FormKey = submission.FormKey,
                FormType = submission.FormType,
                Data = updatedData.Count > 0 ? updatedData : new Dictionary<string, object?>(submission.FieldValues)
            };
        }

        private static object? ConvertValue(object rawValue, Type targetType)
        {
            if (rawValue == null)
            {
                return null;
            }

            Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (underlyingType == typeof(string))
            {
                return rawValue.ToString();
            }

            if (underlyingType == typeof(int))
            {
                if (int.TryParse(rawValue.ToString(), out int parsedInt))
                {
                    return parsedInt;
                }
                return 0;
            }

            if (underlyingType == typeof(bool))
            {
                if (bool.TryParse(rawValue.ToString(), out bool parsedBool))
                {
                    return parsedBool;
                }
                return false;
            }

            if (underlyingType == typeof(Guid))
            {
                if (Guid.TryParse(rawValue.ToString(), out Guid parsedGuid))
                {
                    return parsedGuid;
                }
                return Guid.Empty;
            }

            try
            {
                return Convert.ChangeType(rawValue.ToString(), underlyingType);
            }
            catch
            {
                return rawValue;
            }
        }
    }
}
