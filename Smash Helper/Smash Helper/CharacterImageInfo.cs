using System.IO;

namespace FG_Stream_Helper
{
    public struct CharacterImageInfo
    {
        public int ID { get; set; }
        public string Photo { get; set; }
        public string Name { get; set; }
        public FileInfo fileInfo { get; set; }
    }
}
