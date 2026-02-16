# Funda Makelaar Analytics API

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Analyze Funda real estate listings and rank top makelaars (real estate agents) in the chosen city.

---

## 📋 Overview

**Objective:** Determine which makelaars in Amsterdam have the most properties listed for sale.

**Requirements:**
1. Top 10 makelaars in Amsterdam (all properties for sale)
2. Top 10 makelaars in Amsterdam with properties that have a garden (tuin)

**Constraints:**
- Funda API rate limit: 100 requests/minute
- Must mitigate errors AND handle errors that occur
- Demonstrate separation of concerns
- Code must be testable

---

## 🏗️ Architecture

This solution implements **Hexagonal Architecture** (Ports & Adapters):

**Design Principles:**
- ✅ **Separation of concerns** - Hexagonal architecture with clear boundaries
- ✅ **Dependency inversion** - Ports & adapters pattern
- ✅ **Single responsibility** - Each layer has one reason to change
- ✅ **Testability** - Interfaces and dependency injection throughout

---

## ⚙️ Configuration

**appsettings.json:**
```json
{
  "FundaSettings": {
    "BaseUrl": "https://partnerapi.funda.nl",
    "ApiKey": "YOUR_API_KEY",
    "Resilience": {
      "RetryCount": 3,
      "RetryDelayMilliseconds": 60000,
      "TimeoutMilliseconds": 30000
    }
  }
}
```

**Settings can be overridden via:**
- Environment variables: `FundaSettings__ApiKey=...`
- User secrets (local development)
- Azure Key Vault (production)

---

## 🔧 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### 🏃‍♀️Run application locally

```bash
# Clone repository
git clone https://github.com/sheriljos/property-manager-api
cd property-manager-api

# Restore dependencies
dotnet restore

# Run API
dotnet run --project src/PropertyManager.Api
```
---

## 📡 API Documentation

### Endpoint: Get Top Makelaars

**HTTP Method:** `GET /makelaars`

**Query Parameters:**

| Parameter | Type | Required | Default | Description                                       |
|-----------|------|----------|---------|---------------------------------------------------|
| `city` | string | ✅ Yes | -       | City name (e.g., "amsterdam")                     |
| `filters` | string[] | No | `[]`    | Additional filters (e.g., "tuin", "woonhuis")     |
| `type` | enum | No | `koop`  | Property type: `koop` (purchase) or `huur` (rent) |
| `top` | integer | No | `10`    | Number of results (1-100)                         |

### Examples

**1. Top 10 makelaars in Amsterdam (all properties):**
```bash
GET /makelaars?city=amsterdam&top=10
```

**2. Top 10 makelaars in Amsterdam with garden:**
```bash
GET /makelaars?city=amsterdam&filters=tuin&top=10
```

**3. Top 5 makelaars in Rotterdam with multiple filters:**
```bash
GET /makelaars?city=rotterdam&filters=tuin&filters=woonhuis&top=5
```

### Response Format
```json
[
  {
    "id": 123,
    "name": "ABC Makelaars",
    "NumberOfProperties": 9
  },
  {
    "id": 456,
    "name": "XYZ Real Estate",
    "NumberOfProperties": 7
  },
  {
    "id": 789,
    "name": "Best Homes",
    "NumberOfProperties": 6
  }
]
```

### HTTP Status Codes

| Code | Description |
|------|-------------|
| `200 OK` | Successfully retrieved rankings |
| `400 Bad Request` | Invalid parameters (e.g., missing city) |
| `500 Internal Server Error` | Server error occurred |

---

## 🛡️ Error Handling & Resilience

**Polly Resilience Policies:**

##### Rate Limit Retry Policy
```csharp
// Handles HTTP 429 (Too Many Requests)
.OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
.WaitAndRetryAsync(
    retryCount: 3,
    sleepDurationProvider: _ => TimeSpan.FromSeconds(60)  // Fixed 60s (API reset window)
)
```

**Why fixed 60s?** Funda API rate limit resets every 60 seconds. Exponential backoff would waste retries on delays shorter than the reset period.

##### Transient Error Retry Policy
```csharp
// Handles 5xx errors, network failures, timeouts
.HandleTransientHttpError()
.Or<TimeoutRejectedException>()
.WaitAndRetryAsync(
    retryCount: 3,
    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))  // Exponential: 2s, 4s, 8s
)
```

##### Timeout Policy
```csharp
Policy.TimeoutAsync<HttpResponseMessage>(
    timeout: TimeSpan.FromSeconds(30)  // Prevent hanging requests
)
```

### Error Flow Example
```
Request → 429 Rate Limit
  ↓
Rate Limit Policy catches → Wait 60s → Retry (1/3)
  ↓
Request → 503 Service Unavailable
  ↓
Transient Error Policy catches → Wait 2s → Retry (1/3)
  ↓
Request → Success ✅
```

---

## 🧪 Testing

### Test Coverage

**Business Logic Tests:**
- ✅ Ranking and grouping correctness
- ✅ Top N filtering
- ✅ Edge cases (empty lists, equal counts, single items)
- ✅ Ordering (descending by count)
- ✅ Grouping by makelaar ID (not name)

## 🤖 AI Usage Disclosure
For transparency on AI tool usage:
- ✅ Code generation (e.g., Resilience policies)
- ✅ Documentation writing (README structure, API docs)

### Used AI For:
- ✅ Polly policy pattern validation
- ✅ README documentation structure
- ✅ Best practice verification (hexagonal architecture, IOptions pattern)

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👤 Author

**Your Name**
- GitHub: [@sheriljos](https://github.com/sheriljos)
- LinkedIn: [Sheril Jose](https://www.linkedin.com/in/sheriljose/)

---

## 🙏 Acknowledgments

- **Funda** for providing the Partner API
- **.NET Community** for excellent documentation and libraries

---

**Last Updated:** February 2026