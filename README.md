# TurnitoAPI

API for managing general appointments for companies.

## Stack & Requirements

.NET 10 · ASP.NET Core · EF Core + PostgreSQL 15+ · JWT · Scalar

## How does it work?

User accounts dispose of quotas for registering Companies, Providers, Services and Appointments (Check section "Quotas" for more information). 
You may use it at your own limits, the most basic usage of the API comes under a "Basic" package, where users are allowed 
- 1 Company
- 5 Providers
- 15 services
- 200 appointments

Cleaning appointments after attending is highly advised in order to avoid hitting limits.

## How do i set it up?

1 - Clone the repo
2 - Setup your secrets (settings section under)
3 - Run a migration with `dotnet ef migrations add InitialCreate > dotnet ef database update`
4 - Run the project `dotnet run`
Documentation may be found under /scalar/v1 (only in development mode)

## Settings
Create an appsettings.json in the root folder with the following structure

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=Heregoesyourhost;Database=HeregoesyourDBname;Username=Heregoesyourusername;Password=Heregoesyourpassword"
  },
  "Jwt": {
    "Key": "Here goes your HMAC key (at least 32 digits)",
    "Issuer": "Here goes your issuer name",
    "Audience": "Here goes your audience name",
    "ExpiresInMinutes": "Here goes your token expiration"
  }
}

Set up your secrets with: `dotnet user-secrets set "Jwt:Key" "..."`

## Database Diagram

(Work in progress)

## Auth

POST /api/auth/register
POST /api/auth/login

## Quotas
These are the tokens consumed by users to create new resources such as companies, services, providers and appointments. 
AvailableProviders
AvailableCompanies
AvailableServices
AvailableAppointments

Deleting a resource gives the tokens of such categories back, you may delete "on cascade" or a particular element of your liking [Note that only you can delete oncascade on elements that are higher on the hierarchy, check the Database Diagram section for more information]. 
i.e > 
(Normal deletion): Deleting a Provider will only remove it from the DB and give back an AvailableProvider token
(OnCascade deletion): Deleting a Provider on cascade will trigger a deletion of incoming appointments made under such provider, giving back an AvailableProvider token and a corresponding number of AvailableAppointments.

## Exceptions (will be changed soon into a less resource intensive model)
| Code | When to use |
| 400 | Validation / BussinessRule |
| 401 | Invalid or lacking token |
| 403 | Not an owner of such resource |
| 404 | Not found |
| 409 | Conflicts  |
| 429 | Rate limit |
