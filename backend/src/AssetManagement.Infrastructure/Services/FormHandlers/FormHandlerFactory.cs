using System;
using System.Collections.Generic;
using System.Linq;
using AssetManagement.Core.Services;

namespace AssetManagement.Infrastructure.Services.FormHandlers
{
    public class FormHandlerFactory
    {
        private readonly IEnumerable<IFormTypeHandler> _handlers;

        public FormHandlerFactory(IEnumerable<IFormTypeHandler> handlers)
        {
            _handlers = handlers;
        }

        public IFormTypeHandler GetHandler(string formType)
        {
            string normalizedType = formType?.ToLowerInvariant() ?? "standard";
            IFormTypeHandler? handler = _handlers.FirstOrDefault(h => h.FormType.Equals(normalizedType, StringComparison.OrdinalIgnoreCase));
            return handler ?? _handlers.First(h => h.FormType.Equals("standard", StringComparison.OrdinalIgnoreCase));
        }
    }
}
