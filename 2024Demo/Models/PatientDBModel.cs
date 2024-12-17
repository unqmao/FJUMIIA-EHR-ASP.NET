namespace _2024Demo.Models
{
    /*
          DB Model 是直接對應到你的資料庫中的資料表的模型，意思就是從後端要存取資料庫用的。
     */
    public class PatientDBModel
    {
        public long PatientId {  get; set; }
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
