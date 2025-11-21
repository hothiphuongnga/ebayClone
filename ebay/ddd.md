## DDD - Domain Driven Design
- Domain
    Entities : phản chiếu table trong DB
    Repository: interface : nghiệp vụ cần có chứ không có logic code 
- Application
    - Interface : IService
        Iorderservice
        ....

    - Service : 
        orderservice ...
    - DTOs
        ...
    

- Infrastructure    
    - Repository : thực thi interface trên domain
        OrderRepo
    - UnitOfWork

    - ExternalService
        - EmailService
        - FileService 


