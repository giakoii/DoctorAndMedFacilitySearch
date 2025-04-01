using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.ViewModels
{
    public class MedicalFileViewModel
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public DateTime UploadedAt { get; set; }
        public byte[] FileContent { get; set; }
    }
}
