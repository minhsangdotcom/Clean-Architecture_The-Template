#

[English](README.md) | [Vietnamese](README-VIETNAMESE.md)

#

# Mô Hình Kiến Trúc Sạch (Clean Architecture)

Template này được thiết kế dành cho các bạn backend làm việc với ASP.NET Core. Nó cung cấp một cách hiệu quả để xây dựng các ứng dụng enterprise một cách đơn giản bằng cách tận dụng lợi thế từ kiến trúc Clean Architecture và .NET Core framework.

Với template này, bạn sẽ có được zero configuration, không cần quan tâm đến cấu trúc, cài đặt, môi trường hoặc các thông lệ tốt nhất cho web API, vì tất cả đã được thiết lập :smiley:.

# Cho mình 1 ⭐ nhé

Nếu bạn thấy template này hữu ích và học được điều gì đó từ nó, hãy cân nhắc cho mình một :star:.

Sự hỗ trợ của bạn là động lực giúp mình mang đến những tính năng mới và cải tiến tốt hơn trong các phiên bản sắp tới.

# Định Nghĩa

Clean Architecture là một triết lý thiết kế phần mềm được giới thiệu bởi Robert C. Martin (Uncle Bob). Nó nhấn mạnh việc tách biệt các mối quan tâm và khuyến khích việc tổ chức mã thành các lớp, mỗi lớp có trách nhiệm riêng biệt. Mục tiêu chính của kiến trúc là tạo ra các hệ thống không phụ thuộc vào framework, giao diện người dùng, cơ sở dữ liệu và các cơ quan bên ngoài, đảm bảo tính linh hoạt, khả năng mở rộng và dễ dàng kiểm thử.

Tại phần trung tâm, Clean Architecture tổ chức mã thành các vòng tròn đồng tâm, với mỗi lớp đều có mục đích cụ thể.

![Alt text](Screenshots/clean-architecture.png "Cấu trúc chung của Clean Architecture")

Quy tắc phụ thuộc nói rằng các thành phần phụ thuộc hướng từ ngoài vào trong, đảm bảo rằng các tầng bên trong vẫn tách biệt với các tầng bên ngoài.

### Lợi ích

- **_Các thành phần tách biệt_**: Mỗi một tầng chịu trách nhiệm cho một khía cạnh của ứng dụng, giúp mã dễ hiểu và bảo trì.
- **_Dễ dàng kiểm thử_**: Các business logic được tách biệt khỏi framework và UI, việc kiểm thử đơn vị trở nên đơn giản và đáng tin cậy hơn.
- **_Linh hoạt và Thích nghi_**: Khi thay đổi framework, cơ sở dữ liệu hoặc các hệ thống bên ngoài ít ảnh hưởng đến logic của phần core.
- **_Tái sử dụng_**: Các Business rules có thể được tái sử dụng trong các ứng dụng hoặc hệ thống khác mà không phải thay đổi quá nhiều code.
- **_Khả năng mở rộng_**: Cấu trúc rõ ràng hỗ trợ việc phát triển và thêm tính năng mới mà không cần tái cơ cấu lại.
- **_Không phụ thuộc vào framework_**: Không bị phụ thuộc nhiều vào framework, rất dễ dàng để thanh đổi công nghệ mới.

### Nhược điểm

- **_Phức tạp_**: Cấu trúc các tầng có thể tăng tính phức tạp, đặc biệt đối với các dự án nhỏ nơi các kiến trúc đơn giản hơn có thể phù hợp hơn
- **_Chi phí khởi đầu cao_**: Thiết lập Kiến Trúc Sạch yêu cầu thêm nỗ lực để tổ chức các tầng và tuân theo các nguyên tắc thiết kế nghiêm ngặt.
- **_Khó khăn khi học tập_**: Các developer không quen thuộc với nguyên tắc này có thể mất thời gian để hiểu rõ cấu trúc và lợi ích của nó.
- **_Nguy cơ về cấu trúc quá phức tạp_**: Đối với các ứng dụng nhỏ, các tầng bổ sung có thể không cần thiết và dẫn đến sự phức tạp hóa.
- **_Hiệu năng bị suy giảm_**: Sự trích dẫn và trừa tượng(interface) giữa các tầng có thể giảm hiệu năng, tuy nhiên thường là không đáng kể.

# Features :rocket:

Có gì đặc biệt khiến cho template này trở nên khác biệt so với những template khác có trên Github?

Nó không chỉ có một cấu trúc rất hiện đại dễ dàng mở rộng và duy trì, mà còn có một loại các tính năng, design pattern vô cùng cực hữa ích đặc biệt là cho .NET Core Web API,

Giúp cho bạn làm project của mình mà ít tốn công sức nhất.

Nào hãy cùng mình khám phá nha :

1. [Authentication với JWT](src/Infrastructure/Services/Identity/)
1. [Authorization bằng Vai trò và quyền](#authorize)
1. [Dynamic Search](src/Contracts/Extensions/QueryExtensions/SearchExtensions.cs), [Dynamic Sort](src/Contracts/Extensions/QueryExtensions/SortExtension.cs), [Dynamic Filter](#filtering),[Offset and Cursor Pagination](src/Contracts/Extensions/QueryExtensions/PaginationExtension.cs)
1. [Lưu trữ media bằng AWS S3](src/Infrastructure/Services/Aws/)
1. [Tích hợp sẳn Elastic Search](src/Infrastructure/Services/Elastics/)
1. [Tích Hợp Domain Event](src/Application//Common/DomainEventHandlers/)
1. [Cross-cutting Concerns](src/Application/Common/Behaviors/)
1. [Distributed cache by Redis](src/Infrastructure/Services/DistributedCache/RedisCacheService.cs)
1. [Xử lý bất đồng bộ nhiều Request cùng lúc bằng hàng đợi (ví dụ ở nhánh feature/TicketSale)](src/Infrastructure/Services/DistributedCache/)
1. [Gửi Email](src/Infrastructure/Services/Mail/)
1. [Tích hợp Schedule jobs bằng Hangfire](src/Infrastructure/Services/Hangfires/)
1. [Specification Pattern](src/Domain/Common/Specs/), [Uit of work and Repository pattern](src/Infrastructure/UnitOfWorks/), [Cached repository with decorator design pattern](src/Infrastructure/UnitOfWorks/CachedRepositories/)
1. [Subcutaneous Test](tests/Application.SubcutaneousTests/)
1. [Tự động dịch message](src/Contracts/Common/Messages/)
1. [Mã nguồn mở và Cấp phép MIT](#license)

# Nhá hàng cho các tính năng :fire:

### Authentication

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

### Dynamic search and sort

```
http://localhost:8080/api/Users?PageSize=2&Search.Keyword=N%E1%BA%B5ng&Search.Targets=province.name&Sort=dayOfBirth
```

![search and sort feature](Screenshots/search-sort.png)

### Cross-cutting concerns

![Cross-cutting Concerns feature](Screenshots/crosscutting-concern.png)

### Lưu trử file media bằng Minio

![AWS s3 feature](Screenshots/AWS_S3_Feature.png)

### Tự động dịch message

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

# Sơ lượt về Cấu trúc :mag_right:

**_Domain_**: Tầng Domain đóng vai trò như phần trung tâm trong Clean Architecture, các thành phần bao gồm:

- Aggregates : Là nhóm các entity có mối liên quan với nhau, các value object, enum, interface và Specification pattern (tùy chọn) các bạn có thể đọc thêm ở [https://github.com/ardalis/Specification](https://github.com/ardalis/Specification). Nó có một số nguyên tắc bắt buộc trong quá trình tương tác giữa các root và các thành phần quan hệ của nó và còn nhiều thức khác.
- Exceptions : Tạo ra custom exception cho tầng Domain

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

_Nó không hề phụ thuộc vào bất kể layer nào_

**_Application_**: Tầng ứng dụng đóng vai trò quang trọng trong clean architecture, Nó chứa các business logic, business rule cho ứng dụng và có các thành phần cấu thành như sau:

- <ins>Thư mục Common</ins>:
  - Behaviors : Nơi chứa các cross-cutting concern có thể kể đến như : error logging, validation, performance logging...
  - DomainEventHandler: Nơi implemnet các logic cho gửi event nội bộ.
  - Exceptions: Chứa các exception cho tầng Application.
  - Interfaces: Tạo ra các interfaces cho repositories và các services bên ngoài.
  - Mapping: Chứa các mapping object.
- <ins>Thư mục Features</ins>: Gom nhóm các modules với command/queries sử dụng CQRS pattern và MediaR

  - Common : Đây là nơi mà mình đặt những thứ chung của tất cả các module lại với nhau để dễ dàng cho việc tái sử dụng như là Mapping với Automapper, Request, Response

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

_Chỉ phụ thuộc vào tầng Domain_

**_Infrastucture_** : Tầng Infrastucture là nơi chứa các kết nối với database và các server bên thứ 3, nó có chứa một số thành phần sau đây:

- <ins>Thư mục Data</ins>:
  - Configurations : Chứa các tùy chỉnh cho các entity ở tầng Domain.
  - Interceptors : Nơi chứa các hành động trước và sau khi EF Core thực hiện lưu các thay đổi
  - Migrations: Chứa các file migration cho các tiếp cận bằng code first trong EF.
- Services : Nơi implement các interface ở tầng Application.
- UnitOfWorks: Nơi implement các repository interface ở tầng Application.

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
   ├── 📁 UnitofWork\

_Phụ thuộc vào tầng Application và Domain_

**_Api_**: Chứa các Api endpoint.

    📁 Api\
        ├── 📁 Converters\
        ├── 📁 Endpoints\
        ├── 📁 Extensions\
        ├── 📁 Middlewares\
        ├── 📁 Resources\
        ├── 📁 Settings\
        ├── 📁 wwwroot\

_Phụ thuộc vào tầng Application and Infrastructure_

**_Contract_** : Chứa shared components qua các tầng

# Bắt đầu thôi nào

## Cách để chạy ứng dụng

Các thứ cần để chạy ứng dụng:

- [Net 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker](https://www.docker.com/)

Bước thứ 1 :point_up: :

Copy nội dung trong appsettings.example.json vào appsettings.json.

Chỉnh sửa connection string của PostgreSQL (Bởi vì template này đang sử dụng PostgreSQL).

```json
"DatabaseSettings": {
    "DatabaseConnection": "Host=localhost;Username=[your_username];Password=[your_password];Database=example"
},
```

Nếu các bạn muốn sử dụng các database khác thì chỉ cần chỉnh lại một số đoạn code nhỏ ở DependencyInjection.cs trong Infrastructure.

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

Chỉ cần thay thế UseNpgsql với bất kể database nào mà bạn muốn :smile:.

Sau đó đi tới Data, vào file DesignTimeDbContextFactory

```
builder.UseNpgsql(connectionString);
```

Thay thế như file DependencyInjection.cs ở trên :point_up_2:.

Bước tiếp theo nha :point_right::

```
cd Dockers/MinioS3

```

Đổi tên username và password ở file .env nếu cần thiết, lát nữa các bạn sẽ dùng nó để đăng nhập vào web manager đó.

```
MINIO_ROOT_USER=the_template_storage
MINIO_ROOT_PASSWORD=storage@the_template1`

```

Dùng lệnh sau đây để chạy Amazon S3 service

```
docker-compose up -d

```

Đây là một cách khá hay để sử dụng AWS miễn phí với máy tính của bạn :dollar: Tui đã học được cách này lúc còn ở công ty cũ :pray:

_Mà nè nếu mấy fen đã có sẳn con AWS rồi thì khỏi cần làm mấy cái này nha_

Chỉnh lại setting ở your appsettings.json

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

Các bạn có thể tạo ra cặp access và Secret key bằng giao diện ở [http://localhost:9001](http://localhost:9001)

Bước cuối nha

```
cd src/Api
dotnet run

```

vào swagger ui ở "localhost:8080/docs"

Xong rồi đó :tada: :tada: :tada: :clap:

## Hướng dẫn sử dụng

### Authorize

Để phân quyền cho nó sử dụng AuthorizeBy nha gắn nó vô trên đầu Endpoint (Controller)

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

**_Tạo ra role kèm theo permission_**

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

### Thêm một quyền mới vào ứng dụng

Vào thư mục Constants trong Infrastructure mở file Credential.cs và chú ý tới PermissionGroups

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
```

Chú ý rằng, key là tên của entity cộng thêm "s" và value là danh sách các permission cho entity đó.

Permission được gộp từ hành động và tên entity.
VD:

```
create:user
```

Đây là nơi để tạo ra các permission từ lớp ActionPermission và ObjectPermission.

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

Tạo ra permission mới sau đó thêm nó vào PermissionGroups dictionary và chạy lại ứng dụng.

<div id='filtering'/>

### Bộ lọc

Để thực hiện tính năng filter, Chúng ta sẽ sử dụng cú pháp LHS Brackets.

LHS là cách để sử dụng các phương thức trong dấu ngoặc vuông cho key

VD:

```
/users?filter[dayOfBirth][$gt]="1990-10-01"
```

Ví dụ này nói rằng hãy lấy ra cho tôi tất cả những người có ngày sinh sau ngày 01 tháng 10 năm 1990

Tất cả các phương thức:

| Operator      | Description                                |
| ------------- | ------------------------------------------ |
| $eq           | So sánh bằng                               |
| $eqi          | So sánh bằng (Không phân biệt hoa thường)  |
| $ne           | Không bằng                                 |
| $nei          | Không bằng (Không phân biệt hoa thường)    |
| $in           | Lọc ra các kết quả Có trong mảng này       |
| $notin        | Lọc ra các kết quả không Có trong mảng này |
| $lt           | Bé hơn                                     |
| $lte          | Bé hơn bằng                                |
| $gt           | Lớn hơn                                    |
| $gte          | Lớn hơn hoặc bằng                          |
| $between      | Kết quả nằm giữa 2 phần tử trong mảng      |
| $notcontains  | không chứa                                 |
| $notcontainsi | không chưa (Không phân biệt hoa thường)    |
| $contains     | chứa                                       |
| $containsi    | chứa (Không phân biệt hoa thường)          |
| $startswith   | phần đầu khớp với                          |
| $endswith     | phần cuối khớp với                         |

Vài VD:

```
GET /api/user?filter[gender][$in][0]=1&filter[gender][$in][1]=2
```

```
GET /api/user?filter[gender][$between][0]=1&filter[gender][$between][1]=2
```

```
GET /api/user?filter[firstName][$contains]=abc
```

Phương thúc $and và $or:

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

Các bạn có thể tìm hiểu thêm ỏ một số link sau đây

[https://docs.strapi.io/dev-docs/api/rest/filters-locale-publication#filtering](https://docs.strapi.io/dev-docs/api/rest/filters-locale-publication#filtering)\
[https://docs.strapi.io/dev-docs/api/rest/filters-locale-publication#complex-filtering](https://docs.strapi.io/dev-docs/api/rest/filters-locale-publication#complex-filtering)\
[https://docs.strapi.io/dev-docs/api/rest/filters-locale-publication#deep-filtering](https://docs.strapi.io/dev-docs/api/rest/filters-locale-publication#deep-filtering)

Mình thiết kế input đầu vào dựa trên [Strapi filter](https://docs.strapi.io/dev-docs/api/rest/filters-locale-publication)

Mình đã nhúng sẳn filter tự động vào tất cả các hàm lấy danh sách ở  lớp Repository

```csharp
    await unitOfWork
        .Repository<User>()
        .CursorPagedListAsync<ListUserResponse>(
            new ListUserSpecification(),
            query.ValidateQuery().ValidateFilter(typeof(ListUserResponse))
        );
```
<div id='pagination'/>

### Phân trang

Offset and cursor pagination được tích hợp sẳn trong template.

Để sử dựng offset pagination thêm dòng sau vào code

```csharp
    await unitOfWork
      .Repository<User>()
      .PagedListAsync<ListUserResponse>(
          new ListUserSpecification(),
          query.ValidateQuery().ValidateFilter(typeof(ListUserResponse)),
          cancellationToken
      );
```

Để sử dụng cursor pagination thêm dòng sau vào code

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

# Công nghệ

- .NET 8
- EntityFramework core 8
- AutoMapper
- Fluent validation
- Medator
- XUnit, FluentAssertion, Respawn
- OpenTelemetry
- Redis
- ElasticSearch
- Serilog

# Hỗ trợ

Nếu như có bất kì vấn đề nào thì cho mình biết qua phần issue nhé.

# Lời cảm ơn

:heart: Cảm ơn mọi người rất nhiều :heart: :pray:.

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

<div id="license"/>

# Cấp phép

Dự án này sử dụng [MIT license](LICENSE)
