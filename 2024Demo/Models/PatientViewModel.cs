namespace _2024Demo.Models
{
    /*
     * 今天資料要從前端進入後端之前，我們可以透過這裡處理一些資料格式
     * 例如用 Regular Expression 處理身份證字號沒有按照格式時，可以直接把資料擋住不進入後端。
        [Required]
        [RegularExpression(@"^([A-Z][1-2][0-9]{8}|[A-Z]{2}[0-9]{8})$", ErrorMessage = "Invalid ID Number format.")]
        public required string IdNo { get; set; }
     */
    public class PatientViewModel
    {
        public long PatientId { get; set; }
        public string IdNo { get; set; }
        public bool Active { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public string Telecom { get; set; }
        public string Gender { get; set; }
        public DateTime Birthday { get; set; }
        public string Address { get; set; }
        public string BloodType { get; set; }  // 新增
        public string EmergencyContact { get; set; }  // 新增
        public string MedicalHistory { get; set; }  // 新增
        public string AllergyInfo { get; set; }
    }
}
