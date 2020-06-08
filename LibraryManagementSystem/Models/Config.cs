using System.Collections.Generic;

namespace LibraryManagementSystem.Models
{
    public class Config
    {
        // 数据库配置
        public string DatabaseType { get; set; } = "Sqlite";
        public string DatabaseDataSource { get; set; } = @".\data.db";
        public string DatabasePort { get; set; } = "3306";
        public string DatabaseUser { get; set; } = "root";
        public string DatabasePassword { get; set; } = "root";
        public string Database { get; set; } = "test";
        public string DatabaseCharset { get; set; } = "utf8";
        public string DatabaseSecurity { get; set; } = "none";
        public string DatabaseAttachs { get; set; } = @"";
        public bool DatabasePooling { get; set; } = true;
        public int DatabaseMinPoolSize { get; set; } = 1;
        // 信誉系统配置
        /// <summary>
        /// 信誉超过(>=)Key,获得Value借书时长(天)
        /// </summary>
        public Dictionary<int, int> CreditReward { get; set; } = new Dictionary<int, int> { { 0, 15 }, { 30, 30 }, { 60, 60 } };
        /// <summary>
        /// 还书时间减最后期限间隔天数超过(>=)Key,减少Value信誉值
        /// </summary>
        public Dictionary<int, int> GiveBackPunishment { get; set; } = new Dictionary<int, int> { { 15, 2 }, { 30, 10 }, { 60, 40 } };
        /// <summary>
        /// 最后期限减还书时间间隔天数超过(>=)Key,增加Value信誉值
        /// </summary>
        public Dictionary<int, int> GiveBackReward { get; set; } = new Dictionary<int, int> { { 0, 1 }, { 30, 5 } };
        /// <summary>
        /// 借阅时长超过(>=)这个值才计算信誉值变动(防止无意义反复借阅刷信誉分)
        /// </summary>
        public int CreditBorrowLimit { get; set; } = 2;
        /// <summary>
        /// 会员初始信誉值
        /// </summary>
        public int InitialCreditValue { get; set; } = 30;

        // 密码加密配置
        /// <summary>
        /// 盐长度(Byte)
        /// </summary>
        public int SaltSize { get; set; } = 16;
        /// <summary>
        /// PBKDF2加密算法类型
        /// </summary>
        public string PBKDF2PrfString { get; set; } = "HMACSHA256";
        /// <summary>
        /// PBKDF2加密算法迭代次数
        /// </summary>
        public int PBKDF2IterationTimes { get; set; } = 1000;
        /// <summary>
        /// PBKDF2加密算法生成长度(Byte)
        /// </summary>
        public int PBKDF2SizeRequested { get; set; } = 32;
    }
}
