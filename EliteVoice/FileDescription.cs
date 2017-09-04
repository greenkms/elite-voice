namespace EliteVoice
{
    internal class FileDescription
    {
        public FileDescription(string name = "", string createDate = "")
        {
            Name = name;
            CreateDate = createDate;
        }

        public string Name { set; get; }
        public string CreateDate { set; get; }
    }
}