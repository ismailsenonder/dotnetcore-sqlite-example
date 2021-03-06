﻿using Microsoft.EntityFrameworkCore;
using SqlLiteExample.IRepositories;
using SqlLiteExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SqlLiteExample.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseModel
    {
        private ExampleDbContext _context;

        public BaseRepository()
        {
            _context = new ExampleDbContext();
        }

        public void Create(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            _context.SaveChanges();
        }

        public TEntity CreateAndGet(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            _context.Set<TEntity>().Remove(entity);
            _context.SaveChanges();
        }

        public IQueryable<TEntity> GetAll()
        {
            return _context.Set<TEntity>().AsNoTracking();
        }

        public TEntity GetById(int id)
        {
            return _context.Set<TEntity>().AsNoTracking().FirstOrDefault(x => x.Id == id);
        }

        public void Update(TEntity entity, bool includesNullableProps = false)
        {
            var originalEntity = _context.Set<TEntity>().Find(entity.Id);

            IList<PropertyInfo> properties = typeof(TEntity).GetProperties().ToList();
            foreach (var property in properties)
            {
                PropertyInfo p = entity.GetType().GetProperty(property.Name);
                var changedValue = p.GetValue(entity);
                if (changedValue != null)
                    p.SetValue(originalEntity, changedValue);

                if (includesNullableProps == true && changedValue == null
                    && p.PropertyType.IsGenericType &&
                    p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    p.SetValue(originalEntity, changedValue);
            }

            _context.Set<TEntity>().Update(originalEntity);
            _context.SaveChanges();
        }
    }
}
