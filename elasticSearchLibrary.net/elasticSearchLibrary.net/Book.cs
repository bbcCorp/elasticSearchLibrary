using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace elasticSearchLibrary.net
{
    public class Book
    {
        [DisplayName("content-id")]
        public String ContentId { get; set; }

        [DisplayName("title")]
        public String  Title { get; set; }

        [DisplayName("author")]
        public String Author { get; set; }

        [DisplayName("genre")]
        public String Genre { get; set; }

        [DisplayName("publish-date")]
        public DateTime PublishDate { get; set; }

    }
}
