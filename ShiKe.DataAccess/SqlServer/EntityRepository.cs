using Microsoft.EntityFrameworkCore;
using ShiKe.Common.JsonModels;
using ShiKe.DataAccess.Common;
using ShiKe.DataAccess.SqlServer;
using ShiKe.DataAccess.SqlServer.Ultilities;
using ShiKe.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace ShiKe.DataAccess.SqlServerr
{
    /// <summary>
    /// 针对 IEntityRepository 的具体实现
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityRepository<T> : IEntityRepository<T> where T : class, IEntityBase, new()
    {
        public EntityDbContext EntitiesContext { get; set; }
        readonly EntityDbContext _EntitiesContext;

        public EntityRepository(EntityDbContext context)
        {
            _EntitiesContext = context;
            EntitiesContext = context;
        }

        public virtual void Save()
        {
            try
            {
                _EntitiesContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // 获取错误信息集合
                var errorMessages = ex.Message;
                var itemErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " Error: ", itemErrorMessage);
                throw new DbUpdateException(exceptionMessage,ex);
            }
        }

        public virtual IQueryable<T> GetAll()
        {
            return _EntitiesContext.Set<T>();
        }

        public virtual IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _EntitiesContext.Set<T>();
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            return query;
        }

        /// <summary>
        ///  第一个是判断的条件，除了提取本身的对象数据集合外，还提取包含根据表达式提取关联的的对象的集合
        /// </summary>
        /// <param name="predicate">需要直接提取关联类集合数据的表达式集合，通过逗号隔开<</param>
        /// <returns></returns>
        public virtual IQueryable<T> GetAllMultiCondition(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _EntitiesContext.Set<T>().Where(predicate);
            foreach (var includePropertie in includeProperties)
            {
                query = query.Include(includePropertie);
            }
            return query;
        }

        public virtual T GetSingle(Guid id)
        {
            return GetAll().FirstOrDefault(x => x.ID == id);
        }

        public virtual T GetSingle(Guid id, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> dbSet = _EntitiesContext.Set<T>();
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    dbSet = dbSet.Include(includeProperty);
                }
            }

            var result = dbSet.FirstOrDefault(x => x.ID == id);
            return result;
        }


        public virtual T GetSingleBy(Expression<Func<T, bool>> predicate)
        {
            return _EntitiesContext.Set<T>().Where(predicate).FirstOrDefault(predicate);
        }

        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _EntitiesContext.Set<T>().Where(predicate);
        }

        public virtual PaginatedList<T> Paginate<TKey>(int pageIndex, int pageSize, Expression<Func<T, TKey>> keySelector)
        {
            return Paginate(pageIndex, pageSize, keySelector, null);
        }

        public virtual PaginatedList<T> PaginateDescend<TKey>(int pageIndex, int pageSize, Expression<Func<T, TKey>> keySelector)
        {
            return PaginateDescend(pageIndex, pageSize, keySelector, null);
        }

        public virtual PaginatedList<T> Paginate<TKey>(int pageIndex, int pageSize, Expression<Func<T, TKey>> keySelector, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = GetAllIncluding(includeProperties).OrderBy(keySelector);
            query = (predicate == null) ? query : query.Where(predicate);
            return query.ToPaginatedList(pageIndex, pageSize);
        }

        public virtual PaginatedList<T> PaginateDescend<TKey>(int pageIndex, int pageSize, Expression<Func<T, TKey>> keySelector, Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = GetAllIncluding(includeProperties).OrderByDescending(keySelector);
            query = (predicate == null) ? query : query.Where(predicate);
            return query.ToPaginatedList(pageIndex, pageSize);
        }

        public virtual PaginatedList<T> Paginate(Expression<Func<T, bool>> predicate, ref ListPageParameter pagePara)
        {
            return Paginate(predicate,ref pagePara,null);
        }

        public virtual PaginatedList<T> Paginate(Expression<Func<T, bool>> predicate, ref ListPageParameter pagePara, params Expression<Func<T, object>>[] includeProperties)
        {
            var pageIndex = 1;
            var pageSize = 18;

            pageIndex = pagePara.PageIndex;
            pageSize = pagePara.PageSize;


            #region 根据属性名称确定排序的属性的 lambda 表达式 

            var sortPropertyName = pagePara.SortProperty;
            var target = Expression.Parameter(typeof(object));
            var type = typeof(T);
            var castTarget = Expression.Convert(target, type);
            var propertyArray = sortPropertyName.Split('.');
            var getPropertyValue = Expression.Property(castTarget, propertyArray[0]);
            for (var i = 0; i < propertyArray.Count(); i++)
            {
                if (i > 0)
                {
                    getPropertyValue = Expression.Property(getPropertyValue, propertyArray[i]);
                }
            }
            var sortExpession = Expression.Lambda<Func<T, object>>(getPropertyValue, target);

            #endregion

            PaginatedList<T> boCollection;
            if (pagePara.SortDesc.ToLower() == "default" || pagePara.SortDesc == "")
            {
                boCollection = Paginate(pageIndex, pageSize, sortExpession, predicate, includeProperties);
            }
            else
            {
                boCollection = PaginateDescend(pageIndex, pageSize, sortExpession, predicate, includeProperties);
            }

            pagePara.PageAmount = boCollection.TotalPageCount;
            pagePara.PageIndex = boCollection.PageIndex;
            pagePara.ObjectAmount = ((predicate == null) ? GetAll() : GetAll().Where(predicate)).Count();

            return boCollection;
        }

        public virtual void Add(T entity)
        {
            _EntitiesContext.Set<T>().Add(entity);
        }

        public virtual void AddAndSave(T entity)
        {
            Add(entity);
            Save();
        }

        public virtual void Edit(T entity)
        {
            //DbEntityEntry dbEntityEntry = _EntitiesContext.Entry(entity);
            _EntitiesContext.Set<T>().Update(entity);
        }

        public virtual void EditAndSave(T entity)
        {
            Edit(entity);
            Save();
        }

        public virtual void EditAndSaveBy(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> newValueExpression)
        {
            var dbSet = _EntitiesContext.Set<T>();
            //dbSet.Where(predicate).Update(newValueExpression);
        }

        public virtual void AddOrEdit(T entity)
        {
            var p = GetAll().FirstOrDefault(x => x.ID == entity.ID);
            if (p == null)
            {
                Add(entity);
            }
            else
            {
                Edit(entity);
            }
        }

        public virtual void AddOrEditAndSave(T entity)
        {
            AddOrEdit(entity);
            Save();
        }

        public virtual void Delete(T entity)
        {
            _EntitiesContext.Set<T>().Remove(entity);
            //DbEntityEntry dbEntityEntry = _EntitiesContext.Entry(entity);
            //dbEntityEntry.State = EntityState.Deleted;
        }

        public virtual void DeleteAndSave(T entity)
        {
            Delete(entity);
            Save();
        }
        public virtual DeleteStatusModel DeleteAndSave(Guid id)
        {
            var result = new DeleteStatusModel() { DeleteSatus = true, Message = "删除操作成功！" };
            var hasIstance = HasInstance(id);
            if (!hasIstance)
            {
                result.DeleteSatus = false;
                result.Message = "不存在所指定的数据，无法执行删除操作！";
            }
            else
            {
                var tobeDeleteItem = GetSingle(id);
                try
                {
                    _EntitiesContext.Set<T>().Remove(tobeDeleteItem);
                    _EntitiesContext.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    result.DeleteSatus = false;
                    result.Message = ex.Message;
                }
            }
            return result;
        }

        public virtual void DeleteAndSaveBy(Expression<Func<T, bool>> predicate)
        {
            var dbSet = _EntitiesContext.Set<T>();
            var toBeDeleteItems=dbSet.Where(predicate);//.Delete();
            foreach(var item in toBeDeleteItems)
            {
                dbSet.Remove(item);
            }

        }

        public virtual DeleteStatus DeleteAndSave(Guid id, List<object> relevanceOperations)
        {
            var deleteStatus = new DeleteStatus();
            var returnStatus = true;
            var returnMessage = "";
            var bo = GetSingle(id);

            if (bo == null)
            {
                returnStatus = false;
                returnMessage = "你所删除的数据不存在，如果确定不是数据逻辑错误原因，请将本情况报告系统管理人员。";
                deleteStatus.Initialize(returnStatus, returnMessage);
            }
            else
            {
                #region 处理关联关系

                var i = 0;
                foreach (var deleteOperationObject in relevanceOperations)
                {
                    var deleteProperty = deleteOperationObject.GetType().GetProperties().Where(pn => pn.Name == "CanDelete").FirstOrDefault();
                    var itCanDelete = (bool) deleteProperty.GetValue(deleteOperationObject);

                    var messageProperty = deleteOperationObject.GetType().GetProperties().Where(pn => pn.Name == "OperationMessage").FirstOrDefault();
                    var messageValue = messageProperty.GetValue(deleteOperationObject) as string;

                    if (!itCanDelete)
                    {
                        returnStatus = false;
                        returnMessage += (i++) + "、" + messageValue + "。\n";
                    }
                }

                #endregion

                if (returnStatus)
                {
                    try
                    {
                        DeleteAndSave(bo);
                        deleteStatus.Initialize(returnStatus, returnMessage);
                    }
                    catch (DbUpdateException)
                    {
                        returnStatus = false;
                        returnMessage = "无法删除所选数据，其信息正被使用，如果确定不是数据逻辑错误原因，请将本情况报告系统管理人员。";
                        deleteStatus.Initialize(returnStatus, returnMessage);
                    }
                }
                else
                {
                    deleteStatus.Initialize(returnStatus, returnMessage);
                }
            }
            return deleteStatus;
        }

        public virtual bool HasInstance(Guid id)
        {
            var dbSet = _EntitiesContext.Set<T>();
            return dbSet.Any(x => x.ID == id);
        }

        public bool HasInstance(Expression<Func<T, bool>> predicate)
        {
            var dbSet = _EntitiesContext.Set<T>();
            return dbSet.Any(predicate);
        }

        //public T1 GetSingleOther<T1>(Guid id)
        //{
        //    var dbSet = _EntitiesContext.Set<T1>();

        //}


        #region 异步方法的具体实现
        public virtual async Task<bool> SaveAsyn()
        {
            await _EntitiesContext.SaveChangesAsync();
            return true;
        }

        public virtual async Task<IQueryable<T>> GetAllAsyn()
        {
            var dbSet = _EntitiesContext.Set<T>();
            var result = await dbSet.ToListAsync();
            return result.AsQueryable<T>();
        }

        public virtual async Task<List<T>> GetAllListAsyn()
        {
            var dbSet = _EntitiesContext.Set<T>();
            var result = await dbSet.ToListAsync();
            return result;
        }

        public virtual async Task<IQueryable<T>> GetAllIncludingAsyn(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _EntitiesContext.Set<T>(); //.Include(includeProperties);
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            var result = await query.ToListAsync();
            return result.AsQueryable();
        }

        public virtual async Task<T> GetSingleAsyn(Guid id)
        {
            var dbSet = _EntitiesContext.Set<T>();
            var result = await dbSet.FirstOrDefaultAsync(x => x.ID == id);
            return result;
        }

        public virtual async Task<T> GetSingleAsyn(Guid id, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> dbSet = _EntitiesContext.Set<T>();
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    dbSet = dbSet.Include(includeProperty);
                }
            }

            var result = await dbSet.FirstOrDefaultAsync(x => x.ID == id);
            return result;
        }

        public virtual async Task<IQueryable<T>> FindByAsyn(Expression<Func<T, bool>> predicate)
        {
            var result= await _EntitiesContext.Set<T>().Where(predicate).ToListAsync();
            return result.AsQueryable();
        }

        public virtual async Task<bool> HasInstanceAsyn(Guid id)
        {
            return await _EntitiesContext.Set<T>().AnyAsync(x => x.ID == id);
        }

        public virtual async Task<bool> HasInstanceAsyn(Expression<Func<T, bool>> predicate)
        {
            return await _EntitiesContext.Set<T>().AnyAsync(predicate);
        }

        public virtual async Task<bool> AddOrEditAndSaveAsyn(T entity)
        {
            var dbSet = _EntitiesContext.Set<T>();
            var hasInstance = await dbSet.AnyAsync(x => x.ID == entity.ID);
            if (hasInstance)
                dbSet.Update(entity);
            else
                await dbSet.AddAsync(entity);
            try
            {
                await _EntitiesContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public virtual async Task<DeleteStatusModel> DeleteAndSaveAsyn(Guid id)
        {
            var result = new DeleteStatusModel() { DeleteSatus = true, Message = "删除操作成功！" };
            var hasIstance = await HasInstanceAsyn(id);
            if(!hasIstance)
            {
                result.DeleteSatus = false;
                result.Message = "不存在所指定的数据，无法执行删除操作！";
            }
            else
            {
                var tobeDeleteItem = await GetSingleAsyn(id);
                try
                {
                    _EntitiesContext.Set<T>().Remove(tobeDeleteItem);
                    _EntitiesContext.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    result.DeleteSatus = false;
                    result.Message = ex.Message;
                }
            }
            return result;
        }

        public virtual async Task<PaginatedList<T>> PaginateAsyn<TKey>(int pageIndex, int pageSize, Expression<Func<T, TKey>> keySelector)
        {
            var result = await PaginateAsyn(pageIndex, pageSize, keySelector, null);
            return result;
        }

        public virtual async Task<PaginatedList<T>> PaginateAsyn<TKey>(
            int pageIndex,
            int pageSize,
            Expression<Func<T, TKey>> keySelector,
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = await GetAllIncludingAsyn(includeProperties);
            query =query.OrderBy(keySelector);
            query = (predicate == null) ? query : query.Where(predicate);
            return query.ToPaginatedList(pageIndex, pageSize);
        }

        #endregion
    }
}