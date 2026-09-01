# Lab 2: Global Error Handling

- Duration: ~1 hour
- Context: You are consultants working with a financial services company that needs consistent and secure error responses from their Web API. Right now, when something goes wrong, the API leaks messy stack traces or inconsistent messages to clients. Your task is to implement global error handling middleware that standardizes how errors are reported.

---

#### Learning Objectives

By the end of this lab, you will be able to:

* Author custom exception handling middleware that intercepts unhandled exceptions early in the request pipeline.
* Standardize error responses by returning a consistent JSON error model and appropriate HTTP status codes.
* Create custom exception types (e.g., `NotFoundException`, `ValidationException`) to represent predictable, domain-specific error conditions.
* Map custom exception types to specific HTTP status codes while treating unknown exceptions as generic 500 Internal Server Errors.
* Adhere to standardized error response formats, such as RFC 7807 Problem Details, to provide machine-readable error information to API clients.

---

#### Starter Project

You are given a simple ASP.NET Core Web API project with one controller:

```
/FinanceApi
  /Controllers
    AccountsController.cs     // Contains dummy endpoints
  /Middleware
    ErrorHandlingMiddleware.cs // TODO: implement
  Program.cs                  // Minimal setup + Swagger
  README.md
```

The `AccountsController` has endpoints that throw exceptions (`NotImplementedException`, `KeyNotFoundException`) to simulate failures.

Note: Ensure the project has a `Properties/launchSettings.json` file configured for Development mode to enable Swagger UI.

---

#### Tasks

#### 1. Test Current Error Behavior (5 min)

Goal: See what happens when the API encounters errors without proper error handling.

* Run the project and open Swagger UI
* Try these endpoints that are designed to throw exceptions:
  * `GET /api/accounts/error` - throws `NotImplementedException`
  * `GET /api/accounts/notfound` - throws `KeyNotFoundException`
  * `GET /api/accounts/invalid` - throws `ArgumentException`
* Observe what happens:
  * What response do you get? (Raw HTML error page? Stack trace?)
  * What status code is returned?
  * Is the error information useful for API consumers?
  * What security issues might this expose?

Checkpoint: What problems do you see with the current error handling? Why is this problematic for a financial services API?

---

#### 2. Create Error Handling Middleware (20 min)

* Implement a new class `ErrorHandlingMiddleware`.
* Use `RequestDelegate` to intercept requests in `InvokeAsync`.
* Wrap the pipeline in a `try/catch`.
* Catch exceptions and return a JSON error response with:

  * `status` (HTTP code)
  * `title` (short message)
  * `detail` (error description, safe for clients)
  * `traceId` (from `HttpContext.TraceIdentifier`)

👉 Hint: Look up “ASP.NET Core custom middleware error handling”.

Checkpoint: When you hit `/api/accounts/error`, you should get a JSON response instead of a raw stack trace.

---

#### 3. Standardize Error Format (15 min)

* Update the middleware to return Problem Details (RFC 7807) format.
* Use the built-in `ProblemDetails` class.
* Include at least: `status`, `title`, `detail`, and `instance` (the request path).

👉 Hint: Search “ASP.NET Core ProblemDetails middleware example”.

Checkpoint: Swagger should now show structured JSON errors for all unhandled exceptions.

---

#### 4. Create Custom Exception Types (15 min)

* Add `NotFoundException` and `ValidationException` classes under `/Exceptions`.
* Throw `NotFoundException` from one dummy endpoint in `AccountsController`.
* Update middleware to map:

  * `NotFoundException` → 404
  * `ValidationException` → 400
  * Any other exception → 500

Checkpoint: Test endpoints and confirm status codes match expectations.

---

#### 5. Reflection & PR (10 min)

Create a pull request with your completed middleware and exception types. In the PR description, answer these reflection prompts:

1. Why is it important to standardize error responses in a public-facing API?
2. What’s the difference between a custom exception and a generic one (like `Exception`)?
3. How does using Problem Details (RFC 7807) help API consumers?

---

#### Stretch Goals

* Add structured logging (e.g., Serilog) to log exception details with a `traceId`.
* Add a `ValidationException` that includes a list of field errors in the `extensions` property of `ProblemDetails`.
* Configure Swagger to show example error responses for endpoints.

---

#### Deliverables

* A working `ErrorHandlingMiddleware` registered in the pipeline.
* Consistent JSON error responses using Problem Details.
* Custom exception types with correct HTTP mappings.
* A pull request with reflective answers.