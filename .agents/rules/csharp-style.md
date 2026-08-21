# C# Coding Style Rules

## 1. One Type Per File
- Every class, record, interface, enum, and struct must be placed in its own individual `.cs` file named after the type (e.g. `LoginRequestDto.cs`, `UserDto.cs`).
- Multiple types must not be declared in a single file.

## 2. Control Flow Braces
- Always use explicit block braces `{ }` for all control flow statements (`if`, `else`, `for`, `foreach`, `while`, `do`, etc.), even when the body consists of a single statement.

## 3. Namespace Scoping
- Always use block-scoped namespacing:
  ```csharp
  namespace AssetManagement.Core.Dtos
  {
      public record LoginRequestDto(...);
  }
  ```
- Do NOT use file-scoped namespaces (`namespace AssetManagement.Core.Dtos;`).

## 4. Explicit Typing (No `var`)
- Always use explicit type declarations instead of implicit `var` (e.g., `UserEntity user = ...`, `string email = ...`, `List<string> list = new();`).
- Avoid using `var` anywhere in variable declarations.

## 5. Use `TypedResults` for API Responses
- Always return strongly typed result types using `TypedResults` for API endpoints (e.g., `TypedResults.Ok(...)`, `TypedResults.BadRequest(...)`, `TypedResults.Unauthorized(...)`, `TypedResults.NotFound(...)`).
- Avoid using legacy `Ok(...)`, `BadRequest(...)`, `Unauthorized(...)` helpers from `ControllerBase` without explicit type safety.

## 6. Swagger & OpenAPI Documentation
- Always enable and configure Swagger / OpenAPI UI in API projects (`Program.cs` + `Swashbuckle.AspNetCore`).
- Always enable XML documentation generation in API `.csproj` files (`<GenerateDocumentationFile>true</GenerateDocumentationFile>`).
- Always annotate API controllers and endpoints with XML comments (`<summary>`, `<param>`, `<returns>`, `<response>`) and OpenAPI attributes (`[ProducesResponseType]`, `[Produces]`).
- Configure JWT Bearer security definitions in Swagger options when authentication is enabled.

## 7. Controller Separation of Concerns
- Authentication operations (e.g., `Login`, `SsoLogin`, `Logout`, `TokenRefresh`) belong exclusively in `AuthController` (`/api/auth`).
- User profile editing and preferences (e.g., `UpdateLanguage`, updating avatar or user details) belong in a dedicated `ProfileController` (`/api/profile`). Do NOT place profile management endpoints inside `AuthController`.

## 8. Profile DTOs & Email Validation
- Profile editing must use dedicated DTOs in individual files (`UpdateProfileDto.cs`, `UpdateEmailDto.cs`).
- Full profile updates (`UpdateProfileDto`) handle `FirstName`, `LastName`, `PreferredLanguage`, `AvatarUrl`, etc.
- Email updates (`UpdateEmailDto`) are handled separately via a dedicated endpoint (`PUT /api/profile/email`) with strict email format validation and database uniqueness checking (`!AnyAsync(...)`).
