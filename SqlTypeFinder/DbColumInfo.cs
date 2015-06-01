
namespace SqlTypeFinder
{
    class DbColumInfo
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public bool AllowNull { get; set; }
    }
}
