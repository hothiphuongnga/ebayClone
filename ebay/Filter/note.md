* Filter
xử lý thấp hhonw middleware 

Vòng đời của filter
 
Request 

Middleware exception handler

router
    Auth filter(OnAuthorization)

    Réource filter (OnResourceExecuting) // có cache ->

    model binding

    action filter 
        - excuting : bắt đầu (OnActionExecuting)
        - action method cho api/ controller tương ứng
        - excuted : kết thúc (OnActionExecuted)
    exception filter (OnException)

    Result filter
        - OnResultExecuting
        - tạo json trả về từ kết quả của action method
        - OnResultExecuted

    Resource filter (OnResourceExecuted) cache 

trả về client




    






