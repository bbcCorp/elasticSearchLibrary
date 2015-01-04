using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace elasticSearchLibrary.Core
{
    public class Book
    {
        [DisplayName("Record ID")]
        public Int32 Id { get; set; }

        [DisplayName("ISBN")]
        public String ContentId { get; set; }

        [DisplayName("Title")]
        public String Title { get; set; }

        [DisplayName("Author")]
        public String Author { get; set; }

        [DisplayName("Genre")]
        public String Genre { get; set; }

        [DisplayName("Published Date")]
        public DateTime PublishDate { get; set; }

        [DisplayName("Overview")]
        public DateTime Overview { get; set; }

    }
}
