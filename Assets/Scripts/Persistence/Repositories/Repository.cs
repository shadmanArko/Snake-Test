using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace GameCode.Persistence.Repositories
{
    [Serializable]
    public abstract class Repository<T> where T : Base
    {
        [HideInInspector] public DataContext context;

        private List<T> Entities => context.Set<T>();

        public T GetById(string id)
        {
            return Entities.FirstOrDefault(e => e.id == id);
        }
        public List<T> GetAll()
        {
            return Entities;
        }
        
        public void Add(T entity)
        {
            Entities.Add(entity);
        }

        public void Modify(T entity)
        {
            for (var i = 0; i < Entities.Count; i++)
            {
                if (Entities[i].id == entity.id)
                {
                    Entities[i] = entity;
                }
            }
        }

        public void Delete(T entity)
        {
            Entities.Remove(entity);
        }

        public async Task Save()
        {
            await context.Save();
        }
    }
    
}