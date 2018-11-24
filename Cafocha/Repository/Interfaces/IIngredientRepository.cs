using Cafocha.Entities;
using System;
using System.Collections.Generic;

namespace Cafocha.Repository.Interfaces
{
    public interface IIngredientRepository : IDisposable
    {
        IEnumerable<Ingredient> GetAllIngredients();
        Ingredient GetIngredientById(string ingredientId);
        void InsertIngredient(Ingredient ingredient);
        void DeleteIngredient(string ingredientId);
        void UpdateIngredient(Ingredient ingredient);
        void Save();
    }
}
