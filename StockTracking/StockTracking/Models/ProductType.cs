//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StockTracking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public partial class ProductType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductType()
        {
            this.Products = new HashSet<Product>();
        }
        [DisplayName("�r�n Tip ID")]
        public int ProductTypeID { get; set; }
        [DisplayName("�r�n Tip Ad�")]
        public string ProductName { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }
    }
}
