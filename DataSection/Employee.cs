//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Clicker.DataSection
{
    using System;
    using System.Collections.Generic;
    
    public partial class Employee
    {
        public Employee()
        {
            this.FilesINBarCodes = new HashSet<FilesINBarCode>();
        }
    
        public long PersoneCode { get; set; }
        public string PersonName { get; set; }
        public string PersonFamily { get; set; }
        public string PersonPost { get; set; }
        public System.Guid FkUserId { get; set; }
        public bool AllowLockFiles { get; set; }
    
        public virtual aspnet_Users aspnet_Users { get; set; }
        public virtual ICollection<FilesINBarCode> FilesINBarCodes { get; set; }
    }
}
