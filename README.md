#

[English](README.md) | [Vietnamese](README-VIETNAMESE.md)

#
![Visual Studio Code](https://img.shields.io/badge/Visual%20Studio%20Code-0078d7.svg?style=for-the-badge&logo=visual-studio-code&logoColor=white)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=for-the-badge)](LICENSE) ![GitHub Release](https://img.shields.io/github/v/release/minhsangdotcom/Clean-Architecture_The-Template?style=for-the-badge&color=orange)
![GitHub Org's stars](https://img.shields.io/github/stars/minhsangdotcom%2FClean-Architecture_The-Template?style=for-the-badge&color=pink)
![GitHub forks](https://img.shields.io/github/forks/minhsangdotcom/Clean-Architecture_The-Template?style=for-the-badge&color=%23f61d9c)

- [1. Language](#1-languages)
- [2. Badges](#2-badge)
- [3. Table of Contents](#3-table-of-contents)
- [2. .NET Core Clean Architecture The Template Introduction](#2-net-core-clean-architecture-the-template-introduction)
- [3. Give a Star! ⭐](#3-give-a-star-)
- [4. What is Clean Architecture?](#4-what-is-clean-architecture)
    - [5 Advandtage](#5-advandtage)
    - [5.0.1. Disadvandtage](#501-disadvandtage)
- [6. Features :rocket:](#6-features-rocket)
- [7. Demo :fire:](#7-demo-fire)
    - [7.0.1. Authentication](#701-authentication)
    - [8. Dynamic search and sort](#8-dynamic-search-and-sort)
    - [8.0.3. Cross-cutting concerns](#803-cross-cutting-concerns)
    - [8.0.4. AWS S3 by Minio](#804-aws-s3-by-minio)
    - [8.0.5. Automatic Translatable Message](#805-automatic-translatable-message)
- [9. Structure Overview :mag\_right:](#9-structure-overview-mag_right)
- [10. Getting started](#10-getting-started)
  - [10.1. Run .NET Core Clean Architecture Project](#101-run-net-core-clean-architecture-project)
  - [10.2. Basic Usage](#102-basic-usage)
    - [10.2.1. Authorize](#1021-authorize)
    - [10.2.2. Create role with permissions:](#1022-create-role-with-permissions)
    - [10.2.3. How to add new permissions in your app](#1023-how-to-add-new-permissions-in-your-app)
    - [10.2.4. Filtering](#1024-filtering)
    - [10.2.5. Pagination](#1025-pagination)
- [11. Technology](#11-technology)
- [12. Support](#12-support)
- [13. Credits](#13-credits)
- [14. Licence](#14-licence)


# 2. .NET Core Clean Architecture The Template Introduction

This template is designed for backend developer working with ASP.NET Core. It provides you an efficient way to build enterprise applications effortlessly by leveraging advantages of clean architecture structre and .NET Core framework.

With this template, You'll benefit from zero configuration, and don't need to worry about struture, settings, environments or best practices for web APIs, because everything is already set up :smiley:.

# 3. Give a Star! ⭐

If you find this template helpful and learn something from it, please consider giving it a :star:.

Your support motivates me to deliver even better features and improvements in future versions.

# 4. What is Clean Architecture?

Clean Architecture is a software design philosophy introduced by Robert C. Martin (Uncle Bob). It emphasizes the separation of concerns and promotes the organization of code into layers, each with distinct responsibilities. The architecture's primary goal is to create systems that are independent of frameworks, UI, databases, and external agencies, allowing flexibility, scalability, and testability.

At its core, Clean Architecture organizes code into concentric circles, with each layer having a specific purpose.

![Alt text](Screenshots/clean-architecture.png "clean architecture common structure")

The dependency rule states that code dependencies should only point inward, ensuring that inner layers remain isolated from external layers.

### 5 Advandtage

- **_Seperation of Concerns_**: Each layer is responsible for a specific aspect of the application, making the code easier to understand and maintain.
- **_Testability_**: Since business logic is decoupled from frameworks and UI, unit testing becomes simpler and more reliable.
- **_Flexibility and Adaptability_**: Changes to the framework, database, or external systems have minimal impact on the core logic.
- **_Reusability_**: Business rules can be reused across different applications or systems with minimal changes.
- **_Scalability_**: The clear structure supports growth and the addition of new features without significant refactoring.
- **_Framework Independence_**: Avoids being locked into a specific framework, making it easier to migrate to newer technologies.

### 5.0.1. Disadvandtage

- **_Complexity_**: The layered structure can add complexity, especially for smaller projects where simpler architectures might suffice.
- **_Initial Overhead_**: Setting up Clean Architecture requires additional effort to organize layers and follow strict design principles.
- **_Learning Curve_**: Developers unfamiliar with the principles may take time to grasp the structure and its benefits.
- **_Over-Engineering Risk_**: For small-scale applications, the additional layers might be unnecessary and lead to over-complication.
- **_Performance Overhead_**: The abstraction and indirection between layers can introduce slight performance trade-offs, though typically negligible.

# 6. Features :rocket:

What makes this Clean Architecture template stand out from the rest?

It not only features a scalable and maintainable structure but also includes a wide range of useful features, design patterns specifically designed for .NET Core Web API.

It helps you to do everything effortlessly.

Let's explore the features:

1. [Authentication with JWT for .NET Core](src/Infrastructure/Services/Identity/)
1. [Authorization by Roles and Permissions](#authorize)
1. [Dynamic Search](src/Contracts/Extensions/QueryExtensions/SearchExtensions.cs), [Dynamic Sort](src/Contracts/Extensions/QueryExtensions/SortExtension.cs) , [Dynamic Filter](#filtering),[Offset and Cursor Pagination](#pagination)
1. [AWS S3 Storage](src/Infrastructure/Services/Aws/)
1. [Elastic Search](src/Infrastructure/Services/Elastics/)
1. [Domain Event](src/Application//Common/DomainEventHandlers/)
1. [Cross-cutting Concerns](src/Application/Common/Behaviors/)
1. [Distributed cache by Redis](src/Infrastructure/Services/DistributedCache/RedisCacheService.cs)
1. [Handling concurrent requests with Queue (example at feature/TicketSale)](src/Infrastructure/Services/DistributedCache/)
1. [Sending Email](src/Infrastructure/Services/Mail/)
1. [Schedule jobs by Hangfire](src/Infrastructure/Services/Hangfires/)
1. [Specification Pattern](src/Domain/Common/Specs/), [Uit of work and Repository pattern](src/Infrastructure/UnitOfWorks/), [Cached repository with decorator design pattern](src/Infrastructure/UnitOfWorks/CachedRepositories/)
1. [Subcutaneous Test](tests/Application.SubcutaneousTests/)
1. [Automactic translatable message](src/Contracts/Common/Messages/)
1. [Open source and MIT license](#licence)

# 7. Demo :fire:

### 7.0.1. Authentication

```json
{
  "results": {
    "user": {
      "firstName": "Chloe",
      "lastName": "Kim",
      "username": "chloe.kim",
      "email": "chloe.kim@gmail.com",
      "phoneNumber": "0925123123",
      "dayOfBirth": "1990-09-30T17:00:00Z",
      "gender": 2,
      "province": null,
      "district": null,
      "commune": null,
      "street": "132 Ham Nghi",
      "avatar": null,
      "status": 1,
      "createdBy": "SYSTEM",
      "updatedBy": null,
      "updatedAt": null,
      "id": "01JD936AXSDNMQ713P5XMVRQDV",
      "createdAt": "2024-12-31T08:15:50Z"
    },
    "tokenType": "Bearer",
    "accessTokenExpiredIn": 3600,
    "token": "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIwMUpEOTM2QVhTRE5NUTcxM1A1WE1WUlFEViIsImV4cCI6MTczNzYxMjk4NH0.GMrQKpoaHcCHoKgV4WDeDPAZy_IEj7kUjh7PQRwTNG8",
    "refresh": "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJmYW1pbHlfaWQiOiJaNmI2M3hQSFUxRUsyVkl5R0YyOGJpWUdNTlh1REFrdiIsInN1YiI6IjAxSkQ5MzZBWFNETk1RNzEzUDVYTVZSUURWIiwiZXhwIjoxNzM3Njk1Nzg0fQ.jZgUpT7hQ0icP7FIp3TUzXfl2I4-O5MWEZ78RlBdCiI"
  },
  "statusCode": 200,
  "message": "SUCCESS"
}
```

### 8. Dynamic search and sort

```
http://localhost:8080/api/Users?PageSize=2&Search.Keyword=N%E1%BA%B5ng&Search.Targets=province.name&Sort=dayOfBirth
```

![search and sort feature](Screenshots/search-sort.png)

### 8.0.3. Cross-cutting concerns

![Cross-cutting Concerns feature](Screenshots/crosscutting-concern.png)

### 8.0.4. AWS S3 by Minio

![AWS s3 feature](Screenshots/AWS_S3_Feature.png)

### 8.0.5. Automatic Translatable Message

```json
{
  "type": "BadRequestException",
  "trace": {
    "traceId": "a8ad0670028620121f51850ce5b6cab5",
    "spanId": "fbf21a1849fdadac"
  },
  "errors": [
    {
      "reasons": [
        {
          "message": "user_password_incorrect",
          "en": "Password of user is incorrect",
          "vi": "Mật khẩu của Người dùng không đúng"
        }
      ]
    }
  ],
  "statusCode": 400,
  "message": "One or several errors have occured"
}
```

# 9. Structure Overview :mag_right:

**_Domain_**: Domain layer serves as the core of clean architecture application and contains key elements such as:

- Aggregates : It's a way to group together related entities, value objects, enums, repository interfaces and Specfication (optional) you can learn about it at [https://github.com/ardalis/Specification](https://github.com/ardalis/Specification). With principles are established to govern the interactions between the aggregate root and its relationship and more.
- Exceptions : Create custom exceptions for Domain layer.

  📁 Domain\
   ├── 📁 Aggregates\
   &emsp;&emsp;&emsp;├── 📁 AuditLogs\
   &emsp;&emsp;&emsp;├── 📁 Regions\
   &emsp;&emsp;&emsp;├── 📁 Roles\
   &emsp;&emsp;&emsp;├── 📁 Users\
   ├── 📁 Common\
   &emsp;&emsp;&emsp;├── 📁 ElasticConfigurations\
   &emsp;&emsp;&emsp;├── 📁 Specs\
   ├── 📁 Exceptions

_it is independent of any external dependencies_

**_Application_**: Application layer play a important role in clean architecture, it contains business logics and rules for your application and consist of key elements such as:

- <ins>Common folder</ins>:
  - Behaviors : Create cross-cutting concerns such as : error logging, validation, performance logging...
  - DomainEventHandler: the implementations of sending domain events.
  - Exceptions: Contain exceptions for use case.
  - Interfaces: Define interfaces for repositories and external services.
  - Mapping: Create mapping objects.
- <ins>Features folder</ins>: where I group command and query handlers together for using CQRS pattern and MediaR.

  - Common : It's my own style, I place common things of those modules such as Mapping, validations, requests and responses and reuse it across modules.

    📁 Application\
     ├── 📁 Common\
     &emsp;&emsp;&emsp;├── 📁 Auth\
     &emsp;&emsp;&emsp;├── 📁 Behaviors\
     &emsp;&emsp;&emsp;├── 📁 DomainEventHandler\
     &emsp;&emsp;&emsp;├── 📁 Exceptions\
     &emsp;&emsp;&emsp;├── 📁 Interface\
     &emsp;&emsp;&emsp;&emsp;&emsp;├── 📁 Registers\
     &emsp;&emsp;&emsp;&emsp;&emsp;├── 📁 Services\
     &emsp;&emsp;&emsp;&emsp;&emsp;├── 📁 UnitofWorks\
     &emsp;&emsp;&emsp;├── 📁 Mapping\
     &emsp;&emsp;&emsp;├── 📁 QueryStringProcessing\
     &emsp;&emsp;&emsp;├── 📁 Security\
     ├── 📁 Features\
     &emsp;&emsp;&emsp;├── 📁 AuditLogs\
     &emsp;&emsp;&emsp;&emsp;&emsp;├── 📁 Commands\
     &emsp;&emsp;&emsp;&emsp;&emsp;├── 📁 Queries\
     &emsp;&emsp;&emsp;├── 📁 Common\
     &emsp;&emsp;&emsp;&emsp;&emsp;├── 📁 Mapping\
     &emsp;&emsp;&emsp;&emsp;&emsp;├── 📁 Projections\
     &emsp;&emsp;&emsp;&emsp;&emsp;├── 📁 Validators\
     &emsp;&emsp;&emsp;├── 📁 Permissions\
     &emsp;&emsp;&emsp;&emsp;&emsp;├── 📁 Commands\
     &emsp;&emsp;&emsp;&emsp;&emsp;├── 📁 Queries\
     &emsp;&emsp;&emsp;├── 📁 Regions\
     &emsp;&emsp;&emsp;&emsp;&emsp;├── 📁 Commands\
     &emsp;&emsp;&emsp;&emsp;&emsp;├── 📁 Queries\
     &emsp;&emsp;&emsp;├── 📁 Roles\
     &emsp;&emsp;&emsp;&emsp;&emsp;├── 📁 Commands\
     &emsp;&emsp;&emsp;&emsp;&emsp;├── 📁 Queries\
     &emsp;&emsp;&emsp;├── 📁 Users\
     &emsp;&emsp;&emsp;&emsp;&emsp;├── 📁 Commands\
     &emsp;&emsp;&emsp;&emsp;&emsp;├── 📁 Queries\

_It only depends on Domain leyer_

**_Infrastucture_** : The Infrastucture layer is responsible for handling data from external sources, such as databases and web services and Consists of some key elements such as:

- <ins>Data folder</ins>:
  - Configurations : contain configurations for entity at Domain layer.
  - Interceptors : Where I do logic before and after entity framework apply changes, it's an awesome feature that EF Core bring to us.
  - Migrations: contain migration files for code first approach in EF.
- Services : Implement external services
- UnitOfWorks: Do implementations for unit of work and repository at Application layer.

  📁 Infrastructure\
   ├── 📁 Constants\
   ├── 📁 Data\
   &emsp;&emsp;&emsp;├── 📁 Configurations\
   &emsp;&emsp;&emsp;&emsp;&emsp;├── 📁 Identity\
   &emsp;&emsp;&emsp;&emsp;&emsp;├── 📁 Regions\
   &emsp;&emsp;&emsp;&emsp;&emsp;├── :page_facing_up: AuditLogConfiguration.cs\
   &emsp;&emsp;&emsp;&emsp;&emsp;├── :page_facing_up: DeadLetterQueueConfiguration.cs\
   &emsp;&emsp;&emsp;├── 📁 Interceptors\
   &emsp;&emsp;&emsp;├── 📁 Migrations\
   &emsp;&emsp;&emsp;├── 📁 Seeds\
   &emsp;&emsp;&emsp;├── :page_facing_up: DatabaseSettings.cs\
   &emsp;&emsp;&emsp;├── :page_facing_up: DbInitializer.cs\
   &emsp;&emsp;&emsp;├── :page_facing_up: DesignTimeDbContextFactory.cs\
   &emsp;&emsp;&emsp;├── :page_facing_up: RegionDataSeeding.cs\
   &emsp;&emsp;&emsp;├── :page_facing_up: TheDbContext.cs\
   &emsp;&emsp;&emsp;├── :page_facing_up: ValidateDatabaseSetting.cs\
   ├── 📁 Services\
   ├── 📁 UnitofWork

_It depends on Application and Domain layer_

**_Api_**: contains api endpoints and represents for main running project in application.

    📁 Api
        ├── 📁 Converters
        ├── 📁 Endpoints
        ├── 📁 Extensions
        ├── 📁 Middlewares
        ├── 📁 Resources
        ├── 📁 Settings
        ├── 📁 wwwroot

_It depends on Application and Infrastructure layer_

**_Contract_** : Contains shared components for Application, Infrastructure and API layer.

# 10. Getting started

## 10.1. Run .NET Core Clean Architecture Project

The following prerequisites are required to build and run the solution:

- [Net 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker](https://www.docker.com/)

The first step :point_up: :

we're going to copy content of appsettings.example.json to your own appsettings.json.

Modify PostgreSQL connection string (this template is using PostgreSQL currently).

```json
"DatabaseSettings": {
    "DatabaseConnection": "Host=localhost;Username=[your_username];Password=[your_password];Database=example"
},
```

If you want to use difference database then just customize a few things at DependencyInjection.cs in Infrastructure layer

```csharp
    services.AddDbContextPool<TheDbContext>(
            (sp, options) =>
            {
                NpgsqlDataSource npgsqlDataSource = sp.GetRequiredService<NpgsqlDataSource>();
                options
                    .UseNpgsql(npgsqlDataSource)
                    .AddInterceptors(
                        sp.GetRequiredService<UpdateAuditableEntityInterceptor>(),
                        sp.GetRequiredService<DispatchDomainEventInterceptor>()
                    );
            }
    );
```

Simply Replace UseNpgsql with whatever database you want :smile:.

Navigate to Data folder, and then open DesignTimeDbContextFactory file

```
builder.UseNpgsql(connectionString);
```

Replace it as you did above :point_up_2:.

The next step :point_right::

```
cd Dockers/MinioS3

```

change mino username and password at .env if needed and you're gonna use it for logging in Web UI Manager

```
MINIO_ROOT_USER=the_template_storage
MINIO_ROOT_PASSWORD=storage@the_template1`

```

To Run Amazon S3 service for media file storage.

```
docker-compose up -d

```

This is a really good trick for using AWS for free :dollar: that I learned from my previous company :pray:

_Note that If you already have similar one You can skip this step._

Modify this json setting at your appsettings.json

```json
"S3AwsSettings": {
      "ServiceUrl": "[your_host]:9000",
      "AccessKey": "[yours]",
      "SecretKey": "[yours]",
      "BucketName": "the-template-project",
      "PublicUrl": "[your_host]:9000",
      "PreSignedUrlExpirationInMinutes": 1440,
      "Protocol": 1
    },
```

You can create access and secret key pair with Web UI manager at [http://localhost:9001](http://localhost:9001)

The final step

```
cd src/Api
dotnet run
```

"localhost:8080/docs" is swagger UI path

Congrat! you are all set up :tada: :tada: :tada: :clap:

## 10.2. Basic Usage

### 10.2.1. Authorize

To Achieve this, let's add AuthorizeBy attribute on controller

```csharp
    [HttpPost(Router.UserRoute.Users)]
    [SwaggerOperation(Tags = [Router.UserRoute.Tags], Summary = "create User")]
    [AuthorizeBy(permissions: $"{ActionPermission.create}:{ObjectPermission.user}")]
    public override async Task<ActionResult<ApiResponse>> HandleAsync(
        [FromForm] CreateUserCommand request,
        CancellationToken cancellationToken = default
    )
    {
        CreateUserResponse user = await sender.Send(request, cancellationToken);
        return this.Created201(Router.UserRoute.GetRouteName, user.Id, user);
    }
```

### 10.2.2. Create role with permissions:

Json payload is like

```json
{
  "description": "this is super admin role",
  "name": "superAdmin",
  "roleClaims": [
    {
      "claimType": "permission",
      "claimValue": "create:customer"
    },
    {
      "claimType": "permission",
      "claimValue": "update:customer"
    }
  ]
}
```

### 10.2.3. How to add new permissions in your app

To get this, let's navigate to constants folder in Infrastructure layer, then open Credential.cs file and pay your attention on PermissionGroups Dictionary

```csharp
    public static readonly Dictionary<string, string[]> PermissionGroups =
        new()
        {
            {
                nameof(User) + "s",

                [
                    CreatePermission(ActionPermission.create, ObjectPermission.user),
                    CreatePermission(ActionPermission.update, ObjectPermission.user),
                    CreatePermission(ActionPermission.delete, ObjectPermission.user),
                    CreatePermission(ActionPermission.list, ObjectPermission.user),
                    CreatePermission(ActionPermission.detail, ObjectPermission.user),
                ]
            },
            {
                nameof(Role) + "s",

                [
                    CreatePermission(ActionPermission.create, ObjectPermission.role),
                    CreatePermission(ActionPermission.update, ObjectPermission.role),
                    CreatePermission(ActionPermission.delete, ObjectPermission.role),
                    CreatePermission(ActionPermission.list, ObjectPermission.role),
                    CreatePermission(ActionPermission.detail, ObjectPermission.role),
                ]
            },
        };
```

Notice that, the key is your entity name plus "s" and the value is list of permission for that entity.

Permission combibes from action and entity name.
For example:

```
create:user
```

Let's take a look at ActionPermission and ObjectPermission class

```csharp
public static class ActionPermission
{
    public const string create = nameof(create);
    public const string update = nameof(update);
    public const string delete = nameof(delete);
    public const string detail = nameof(detail);
    public const string list = nameof(list);
    public const string testa = nameof(testa);
}

public static class ObjectPermission
{
    public const string user = nameof(user);
    public const string role = nameof(role);
}
```

Define your new one, then push it into PermissionGroups dictionary, and restart application.

### 10.2.4. Filtering

To do filter in this template, we use LHS Brackets.

LHS is the way to encode operators is the use of square brackets [] on the key name.

For example

```
/users?filter[dayOfBirth][$gt]="1990-10-01"
```

This example indicates filtering out users whose birthdays are after 1990/10/01

All support operations:

| Operator      | Description                         |
| ------------- | ----------------------------------- |
| $eq           | Equal                               |
| $eqi          | Equal (case-insensitive)            |
| $ne           | Not equal                           |
| $nei          | Not equal (case-insensitive)        |
| $in           | Included in an array                |
| $notin        | Not included in an array            |
| $lt           | Less than                           |
| $lte          | Less than or equal to               |
| $gt           | Greater than                        |
| $gte          | Greater than or equal to            |
| $between      | Is between                          |
| $notcontains  | Does not contain                    |
| $notcontainsi | Does not contain (case-insensitive) |
| $contains     | Contains                            |
| $containsi    | Contains (case-insensitive)         |
| $startswith   | Starts with                         |
| $endswith     | Ends with                           |

Some Examples:

```
GET /api/user?filter[gender][$in][0]=1&filter[gender][$in][1]=2
```

```
GET /api/user?filter[gender][$between][0]=1&filter[gender][$between][1]=2
```

```
GET /api/user?filter[firstName][$contains]=abc
```

$and and $or operator:

```
/api/users/filter[$and][0][firstName][$containsi]="sa"&filter[$and][1][lastName][$eq]="Tran"
```

```JSON
"filter":{
    "$and": {
        "firstName":"ng",
        "lastName":"Tran"
    }
}
```

```
/api/users/filter[$or][0][$and][0][claims][claimValue][$eq]=admin
```

```JSON
"filter":{
    "$or": {
        "$and":{
            "claims": : {
                "claimValue": "admin"
            }
        }
    }
}
```

For more examples and get better understand, you can visit

[https://docs.strapi.io/dev-docs/api/rest/filters-locale-publication#filtering](https://docs.strapi.io/dev-docs/api/rest/filters-locale-publication#filtering)\
[https://docs.strapi.io/dev-docs/api/rest/filters-locale-publication#complex-filtering](https://docs.strapi.io/dev-docs/api/rest/filters-locale-publication#complex-filtering)\
[https://docs.strapi.io/dev-docs/api/rest/filters-locale-publication#deep-filtering](https://docs.strapi.io/dev-docs/api/rest/filters-locale-publication#deep-filtering)

'Cause I designed filter input based on [Strapi filter](https://docs.strapi.io/dev-docs/api/rest/filters-locale-publication)

To Apply dynamic filter, you just call any list method at Repository class

```csharp
    await unitOfWork
        .Repository<User>()
        .CursorPagedListAsync<ListUserResponse>(
            new ListUserSpecification(),
            query.ValidateQuery().ValidateFilter(typeof(ListUserResponse))
        );
```

### 10.2.5. Pagination

This template supports offset pagination and cursor pagination.

To Enable offset pagination just add this line

```csharp
    await unitOfWork
      .Repository<User>()
      .PagedListAsync<ListUserResponse>(
          new ListUserSpecification(),
          query.ValidateQuery().ValidateFilter(typeof(ListUserResponse)),
          cancellationToken
      );
```

To Enable cursor pagination just add this line

```csharp
     await unitOfWork
        .Repository<User>()
        .CursorPagedListAsync<ListUserResponse>(
            new ListUserSpecification(),
            query.ValidateQuery().ValidateFilter(typeof(ListUserResponse))
        );
```

```json
{
  "results": {
    "data": [
      {
        "firstName": "Sang",
        "lastName": "Tran",
        "username": "sang.tran",
        "email": "sang.tran@gmail.com",
        "phoneNumber": "0925123123",
        "dayOfBirth": "2024-12-31T17:00:00Z",
        "gender": 1,
        "province": {
          "code": "79",
          "name": "Hồ Chí Minh",
          "nameEn": "Ho Chi Minh",
          "fullName": "Thành phố Hồ Chí Minh",
          "fullNameEn": "Ho Chi Minh City",
          "customName": "Thành phố Hồ Chí Minh",
          "createdBy": "SYSTEM",
          "updatedBy": "01JD936AXSDNMQ713P5XMVRQDV",
          "updatedAt": "2024-11-24T05:50:26Z",
          "id": "01JAZDXCWY3Z9K3XS0AYZ733NF",
          "createdAt": "2024-11-09T13:13:27Z"
        },
        "district": {
          "code": "783",
          "name": "Củ Chi",
          "nameEn": "Cu Chi",
          "fullName": "Huyện Củ Chi",
          "fullNameEn": "Cu Chi District",
          "customName": null,
          "createdBy": "SYSTEM",
          "updatedBy": null,
          "updatedAt": null,
          "id": "01JAZDXDGSP0J0XF10836TR3QY",
          "createdAt": "2024-11-09T13:13:27Z"
        },
        "commune": {
          "code": "27505",
          "name": "Trung Lập Thượng",
          "nameEn": "Trung Lap Thuong",
          "fullName": "Xã Trung Lập Thượng",
          "fullNameEn": "Trung Lap Thuong Commune",
          "customName": null,
          "createdBy": "SYSTEM",
          "updatedBy": null,
          "updatedAt": null,
          "id": "01JAZDXEAS1A1RJ4FSTWKW7RJA",
          "createdAt": "2024-11-09T13:13:27Z"
        },
        "street": "abc",
        "avatar": "http:localhost:9000/the-template-project/Users/avatarcute2.1737642177170.jpg?AWSAccessKeyId=bAWMwoigEBePW8tyS4et&Expires=1737896145&Signature=X9c8uoe%2FiGmYZkixo4MdEsXaeog%3D",
        "status": 1,
        "createdBy": "01JD936AXSDNMQ713P5XMVRQDV",
        "updatedBy": null,
        "updatedAt": null,
        "id": "01JJ9RPW9B0FJV39JSNNT5ZKSB",
        "createdAt": "2025-01-23T14:22:56Z"
      }
    ],
    "paging": {
      "pageSize": 1,
      "totalPage": 21,
      "hasNextPage": true,
      "hasPreviousPage": false,
      "cursor": {
        "before": null,
        "after": "9x1HiQ0V+K5Dadbuh3QjfggTc3Ap7o9dgd5FbAJlEsWEyBO33wPBu66g+D0sb26sUncnOcmKHAWlQD5RHDiE44qCV+K11jKIjwOVSFY8XD3OsiA8biRl6dKTNvWNaoYhNh30nNwhHzWTAXQVYunsg0k9gykJWKbSzrI="
      }
    }
  },
  "statusCode": 200,
  "message": "SUCCESS"
}
```


# 11. Technology

- .NET 8
- EntityFramework core 8
- AutoMapper
- Fluent validation
- Medator
- XUnit, FluentAssertion, Respawn
- OpenTelemetry
- PostgreSQL
- Redis
- ElasticSearch
- Serilog
- Docker
- Github Workflow

# 12. Support

If you are having problems, please let me know by issue section.

# 13. Credits

:heart: Thank you guys so much :heart: :pray:.

- [Clean architecture by Jayson Taylor](https://github.com/jasontaylordev/CleanArchitecture)\
  A fantastic guide to structuring projects with clean architecture principles, which helped shape the design of this template.

- [Clean architecture by amantinband](https://github.com/amantinband/clean-architecture)\
  A concise and practical implementation of clean architecture that provided fresh perspectives and further deepened my understanding of this powerful approach.

- [Clean architecture by Ardalis](https://github.com/ardalis/CleanArchitecture)\
  A thoughtful and thorough take on clean architecture that helped refine the modular structure and scalability of this template.

- [Specification pattern](https://github.com/ardalis/Specification)\
  A brilliant implementation of the Specification pattern, simplifying complex query logic and promoting cleaner, more maintainable code.

- [REPR Pattern](https://github.com/ardalis/ApiEndpoints)\
  A practical implementation of the Request-Endpoint-Response (REPR) pattern, which provided a clear and structured approach to organizing API endpoints. It emphasizes simplicity and single-responsibility for each endpoint, ensuring clarity and maintainability.

- [Clean testing by Jayson Taylor](https://github.com/jasontaylordev/CleanArchitecture/tree/main/tests)\
  An inspiring repository of testing best practices, showcasing how to write robust and meaningful tests to ensure the reliability of clean architecture-based projects.

# 14. Licence

This project is licensed with the [MIT license](LICENSE).
