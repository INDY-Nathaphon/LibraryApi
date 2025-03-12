namespace LibraryApi.Controllers.DTO.BaseDTO
{
    public class ResponseDto
    {
        public bool Success { get; set; } // บอกว่า Request สำเร็จหรือไม่
        public string Message { get; set; } // ข้อความแจ้งเตือน/ข้อผิดพลาด
        public object? Data { get; set; } // ข้อมูลเพิ่มเติม (ถ้ามี)

        public ResponseDto(bool success, string message, object? data = null)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }

}
