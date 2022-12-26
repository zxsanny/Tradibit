using Microsoft.EntityFrameworkCore;
using Tradibit.SharedUI.DTO;

namespace Tradibit.DataAccess;

public static class DbContextExtensions
{
    public static async Task<T> Save<T>(this DbContext db, T entity, CancellationToken cancellationToken = default) where T : BaseTrackableId
    {
        if (entity.Id == Guid.Empty)
        {
            var existingEntity = await db.Set<T>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);
            if (existingEntity == null)
                throw new Exception($"No {typeof(T).Name} in db to update with such id {entity.Id}");

            entity.CreatedBy = existingEntity.CreatedBy;
            entity.CreatedDateTime = existingEntity.CreatedDateTime;
            db.Set<T>().Update(entity);
        }
        else
        {
            await db.Set<T>().AddAsync(entity, cancellationToken);
        }

        await db.SaveChangesAsync(cancellationToken);
        return entity;
    }
}