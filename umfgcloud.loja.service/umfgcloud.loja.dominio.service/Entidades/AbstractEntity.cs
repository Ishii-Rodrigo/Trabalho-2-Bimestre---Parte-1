using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace umfgcloud.loja.dominio.service.Entidades
{
    public abstract class AbstractEntity
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string CreatedByUserId { get; private set; } = string.Empty;
        public string CreatedByUserEmail { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        public string UpdatedByUserId { get; private set; } = string.Empty;
        public string UpdatedByUserEmail { get; private set; } = string.Empty;
        public DateTime UpdatedAt { get; private set; } = DateTime.Now;
        public bool IsActive { get; private set; } = true;

        protected AbstractEntity() { }

        protected AbstractEntity(string userId, string userEmail) 
        {
            CreatedByUserId = userId ?? throw new ArgumentNullException(nameof(userId));
            CreatedByUserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
            UpdatedByUserId = userId ?? throw new ArgumentNullException(nameof(userId));
            UpdatedByUserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
        }

        //permite ser feita a sobrecarga de metodo (override), apenas virtual e abstract tem essa funcao
        public virtual void Activate() => IsActive = true;
        public virtual void Inactivate() => IsActive = false;

        public void Update(string userId, string userEmail)
        {
            UpdatedByUserId = userId ?? throw new ArgumentNullException(nameof(userId));
            UpdatedByUserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
            UpdatedAt = DateTime.Now;
        }
    }
}
