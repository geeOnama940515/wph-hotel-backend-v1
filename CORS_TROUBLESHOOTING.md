# CORS Troubleshooting Guide

This guide helps resolve CORS (Cross-Origin Resource Sharing) issues that may occur when the frontend tries to communicate with the backend API.

## 🔍 **Common CORS Issues**

### 1. **Preflight Request Failures**
- **Symptom**: Browser sends OPTIONS request, but server doesn't respond properly
- **Cause**: Missing CORS headers or incorrect middleware order
- **Solution**: Ensure CORS middleware is configured correctly

### 2. **Authentication Interference**
- **Symptom**: CORS works for anonymous endpoints but fails for authenticated ones
- **Cause**: Authentication middleware interfering with CORS preflight
- **Solution**: Configure authentication to allow OPTIONS requests

### 3. **Exception Handling Issues**
- **Symptom**: Unhandled exceptions causing 500 errors that break CORS
- **Cause**: Controllers without try-catch blocks throwing exceptions
- **Solution**: Add global exception handler

## 🛠️ **Current Configuration**

### CORS Policy (Development)
```csharp
options.AddPolicy("AllowDevOrigin", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader()
           .WithExposedHeaders("Content-Disposition", "Content-Length", "Content-Type")
           .SetIsOriginAllowed(origin => true);
});
```

### Middleware Order
```csharp
// Global exception handler
app.Use(async (context, next) => { /* exception handling */ });

// CORS debugging
app.Use(async (context, next) => { /* CORS logging */ });

// CORS middleware
app.UseCors("AllowDevOrigin");

// Authentication
app.UseAuthentication();
app.UseAuthorization();
```

## 🧪 **Testing CORS**

### 1. **Test Endpoints**
Use these endpoints to verify CORS is working:

```bash
# Simple CORS test
curl -X GET http://localhost:5069/api/cors-test

# Room controller CORS test
curl -X GET http://localhost:5069/api/room/cors-test

# Test with preflight
curl -X OPTIONS http://localhost:5069/api/room \
  -H "Origin: http://localhost:3000" \
  -H "Access-Control-Request-Method: GET" \
  -H "Access-Control-Request-Headers: Content-Type"
```

### 2. **Browser Testing**
Open browser console and test:

```javascript
// Test simple GET request
fetch('http://localhost:5069/api/cors-test')
  .then(response => response.json())
  .then(data => console.log(data))
  .catch(error => console.error('CORS Error:', error));

// Test authenticated request
fetch('http://localhost:5069/api/room', {
  headers: {
    'Authorization': 'Bearer your-token-here',
    'Content-Type': 'application/json'
  }
})
.then(response => response.json())
.then(data => console.log(data))
.catch(error => console.error('CORS Error:', error));
```

## 🔧 **Troubleshooting Steps**

### Step 1: Check Middleware Order
Ensure middleware is in the correct order in `Program.cs`:

```csharp
// 1. Exception handling (first)
app.Use(async (context, next) => { /* global exception handler */ });

// 2. CORS (before authentication)
app.UseCors("AllowDevOrigin");

// 3. Authentication (after CORS)
app.UseAuthentication();
app.UseAuthorization();

// 4. Controllers (last)
app.MapControllers();
```

### Step 2: Verify CORS Policy
Check that the CORS policy allows your frontend origin:

```csharp
// For development (allows any origin)
builder.AllowAnyOrigin()
       .AllowAnyMethod()
       .AllowAnyHeader();

// For production (specific origins)
builder.WithOrigins("https://your-frontend-domain.com")
       .AllowAnyMethod()
       .AllowAnyHeader();
```

### Step 3: Check Authentication Configuration
Ensure JWT authentication doesn't interfere with CORS:

```csharp
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Allow HTTP in development
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        // ... other settings
    };
});
```

### Step 4: Test with Different Scenarios

#### Anonymous Endpoints
```bash
# Should work without authentication
curl -X GET http://localhost:5069/api/room
curl -X GET http://localhost:5069/api/room/cors-test
```

#### Authenticated Endpoints
```bash
# Should work with valid token
curl -X GET http://localhost:5069/api/room \
  -H "Authorization: Bearer your-valid-token"
```

#### Preflight Requests
```bash
# Should return 200 with CORS headers
curl -X OPTIONS http://localhost:5069/api/room \
  -H "Origin: http://localhost:3000" \
  -H "Access-Control-Request-Method: POST" \
  -H "Access-Control-Request-Headers: Content-Type,Authorization"
```

## 🐛 **Common Error Messages**

### 1. **"Access to fetch at '...' from origin '...' has been blocked by CORS policy"**
- **Cause**: CORS policy not allowing the origin
- **Solution**: Update CORS policy to include your frontend origin

### 2. **"Response to preflight request doesn't pass access control check"**
- **Cause**: OPTIONS request not handled properly
- **Solution**: Ensure CORS middleware is before authentication

### 3. **"No 'Access-Control-Allow-Origin' header is present"**
- **Cause**: CORS headers not being added
- **Solution**: Check CORS policy configuration

### 4. **"Request header field authorization is not allowed"**
- **Cause**: Authorization header not allowed in CORS policy
- **Solution**: Add `AllowAnyHeader()` to CORS policy

## 🔍 **Debugging Tools**

### 1. **Enable Detailed Logging**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore.Cors": "Debug",
      "Microsoft.AspNetCore.Authentication": "Debug"
    }
  }
}
```

### 2. **Browser Developer Tools**
- Open Network tab
- Look for failed requests (red)
- Check Response headers for CORS headers
- Look for OPTIONS preflight requests

### 3. **Server Logs**
Check server logs for:
- CORS preflight requests
- Authentication failures
- Unhandled exceptions

## 🚀 **Quick Fixes**

### 1. **Reset CORS Configuration**
```csharp
// In DependencyInjection.cs
services.AddCors(options =>
{
    options.AddPolicy("AllowDevOrigin", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
```

### 2. **Add CORS Headers Manually**
```csharp
// In controller (temporary fix)
[HttpOptions]
public IActionResult Options()
{
    Response.Headers.Add("Access-Control-Allow-Origin", "*");
    Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
    Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
    return Ok();
}
```

### 3. **Disable Authentication Temporarily**
```csharp
// Comment out authentication middleware for testing
// app.UseAuthentication();
// app.UseAuthorization();
```

## 📋 **Checklist**

- [ ] CORS middleware is before authentication
- [ ] CORS policy allows your frontend origin
- [ ] Global exception handler is in place
- [ ] JWT configuration is correct
- [ ] OPTIONS requests are handled
- [ ] All required headers are allowed
- [ ] Server logs show no errors
- [ ] Browser console shows no CORS errors

## 🆘 **Still Having Issues?**

1. **Check server logs** for detailed error messages
2. **Test with Postman** to isolate frontend vs backend issues
3. **Verify frontend URL** matches CORS policy
4. **Check for typos** in configuration
5. **Restart the application** after configuration changes

## 📞 **Support**

If you're still experiencing CORS issues after following this guide:
1. Check the server logs for specific error messages
2. Test the endpoints with curl or Postman
3. Verify your frontend is making requests to the correct URL
4. Ensure all configuration changes have been applied and the server restarted 