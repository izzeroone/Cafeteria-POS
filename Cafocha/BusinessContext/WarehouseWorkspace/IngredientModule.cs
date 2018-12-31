using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.BusinessContext.WarehouseWorkspace
{
    public class IngredientModule
    {
        private readonly RepositoryLocator _unitofwork;

        public IngredientModule()
        {
        }

        public IngredientModule(RepositoryLocator unitofwork)
        {
            _unitofwork = unitofwork;
        }

        public IEnumerable<Ingredient> getAllIngredients()
        {
            return _unitofwork.IngredientRepository.Get(x => x.Deleted == 0).ToList();
        }

        public IEnumerable<Ingredient> getAllIngredientsWithWarehouse()
        {
            return _unitofwork.IngredientRepository.Get(x => x.Deleted == 0, includeProperties: "WareHouse").ToList();
        }

        public IEnumerable<Ingredient> getAllIngredientsWithWarehouseWithDeleteOne()
        {
            return _unitofwork.IngredientRepository.Get(includeProperties: "WareHouse").ToList();
        }
        public void insertIngredient(Ingredient ingredient)
        {
            WareHouse newWare = new WareHouse
            {
                WarehouseId = "",
                Contain = 0,
                StdContain = (int)App.Current.Properties["StdContain"]
            };

            _unitofwork.WareHouseRepository.Insert(newWare);
            _unitofwork.Save();
            ingredient.WarehouseId = newWare.WarehouseId;

            _unitofwork.IngredientRepository.Insert(ingredient);
            _unitofwork.Save();
        }

        public void updateIngredient(Ingredient ingredient)
        {
            _unitofwork.IngredientRepository.Update(ingredient);
            _unitofwork.Save();
        }

        public void deleteIngredient(Ingredient ingredient)
        {
            ingredient.Deleted = 1;
            var pdofingre = _unitofwork.ProductDetailsRepository.Get(x => x.IgdId.Equals(ingredient.IgdId)).ToList();
            if (pdofingre.Count != 0)
            {
                foreach (var pd in pdofingre)
                {
                    _unitofwork.ProductDetailsRepository.Delete(pd);
                }
                _unitofwork.Save();
            }

            _unitofwork.IngredientRepository.Update(ingredient);
            _unitofwork.Save();
        }
    }
}
