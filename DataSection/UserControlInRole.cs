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
    
    public partial class UserControlInRole
    {
        public long Id { get; set; }
        public Nullable<int> UserControlId { get; set; }
        public System.Guid RoleId { get; set; }
        public string Description { get; set; }
    
        public virtual aspnet_Roles aspnet_Roles { get; set; }
    }
}
