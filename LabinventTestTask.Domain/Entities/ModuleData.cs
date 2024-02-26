using System.ComponentModel.DataAnnotations;

namespace LabinventTestTask.Domain.Entities
{
    public class ModuleData
    {
        [Key]
        public string ModuleCategoryID { get; set; }
        public string ModuleState { get; set; }

        public ModuleData(string moduleCategoryID, string moduleState)
        {
            ModuleCategoryID = moduleCategoryID;
            ModuleState = moduleState;
        }
    }
}
