using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InsuranceCore.Models
{
    public class AdminRoleDetails
    {
        [Key]
        public int Id { get; set; }
        public int RoleId { get; set; }
        public AdminRoles Role { get; set; }
        public string MenuOption { get; set; }
        public string RoleDescription { get; set; }
        public DateTime? CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public DateTime? DeletionTime { get; set; }
        public long? CreatorUserId { get; set; }
        public long? LastModifierUserId { get; set; }
        public long? DeleterUserId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
