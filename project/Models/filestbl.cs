//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace project.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class filestbl
    {
        public System.Guid fileid { get; set; }
        public string filename { get; set; }
        public byte[] filedata { get; set; }
        public string filesize { get; set; }
        public Nullable<System.DateTime> uploaddate { get; set; }
        public string filehash { get; set; }
        public string uid { get; set; }
        public Nullable<int> blocked { get; set; }
        public Nullable<System.DateTime> lastdownloaddate { get; set; }
        public string sharedwith { get; set; }
        public string cause { get; set; }
    }
}
