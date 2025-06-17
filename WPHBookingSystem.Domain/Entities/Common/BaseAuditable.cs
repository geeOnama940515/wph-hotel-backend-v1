using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPHBookingSystem.Domain.Entities.Common
{
    /// <summary>
    /// Base class for all auditable entities in the domain.
    /// Provides automatic tracking of creation and modification metadata.
    /// This class implements the audit pattern to maintain a complete history
    /// of when and by whom entities were created or modified.
    /// </summary>
    public abstract class BaseAuditable
    {
        /// <summary>
        /// Gets or sets the timestamp when the entity was created.
        /// Automatically initialized to the current UTC time when the entity is instantiated.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Gets or sets the timestamp when the entity was last updated.
        /// Null if the entity has never been modified since creation.
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who created the entity.
        /// This could be a user ID, username, or any other identifier that
        /// uniquely identifies the creator.
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the identifier of the user who last updated the entity.
        /// Null if the entity has never been modified since creation.
        /// This could be a user ID, username, or any other identifier that
        /// uniquely identifies the last modifier.
        /// </summary>
        public string? UpdatedBy { get; set; }
    }
}
