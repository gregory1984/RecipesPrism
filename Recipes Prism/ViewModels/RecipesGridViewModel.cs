using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using System;
using System.Windows;
using System.Windows.Data;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Practices.Unity;
using Recipes_Prism.Events;
using Recipes_Prism.Events.Payloads;
using Recipes_Prism.Events.Pagination;
using Recipes_Prism.Enums;
using Recipes_Prism.Helpers;
using Recipes_Model.Interfaces;
using Recipes_Model.DTO;
using Recipes_Model.DTO.Searching;

namespace Recipes_Prism.ViewModels
{
    public class RecipesGridViewModel : BindableBase
    {
        private string productName = "";
        public string ProductName
        {
            get => productName;
            set
            {
                SetProperty(ref productName, value);
                RecipesView.Refresh();
            }
        }

        private IList<RecipeViewModel> recipes;
        private ICollectionView recipesView;
        public ICollectionView RecipesView
        {
            get => recipesView;
            set => SetProperty(ref recipesView, value);
        }

        private RecipeViewModel selectedRecipe;
        public RecipeViewModel SelectedRecipe
        {
            get => selectedRecipe;
            set => SetProperty(ref selectedRecipe, value);
        }

        private readonly IEventAggregator eventAggregator;
        private readonly IUnityContainer unityContainer;
        private readonly IOrderingService orderingService;

        private WindowMode windowMode;
        private OrderViewModel selectedOrder;

        public RecipesGridViewModel(IEventAggregator eventAggregator, IUnityContainer unityContainer, IOrderingService orderingService)
        {
            this.eventAggregator = eventAggregator;
            this.unityContainer = unityContainer;
            this.orderingService = orderingService;
        }

        private DelegateCommand loaded;
        public DelegateCommand Loaded
        {
            get => loaded ?? (loaded = new DelegateCommand(() =>
            {
                eventAggregator.ExecuteSafety(() =>
                {
                    windowMode = unityContainer.Resolve<WindowMode>(UnityNames.AddEditOrderWindowMode);
                    if (windowMode == WindowMode.Edition)
                    {
                        selectedOrder = unityContainer.Resolve<OrderViewModel>(UnityNames.SelectedOrderForEdition);
                        LoadRecipesOfSelectedOrder();
                    }
                    else recipes = new List<RecipeViewModel>();

                    LoadUndefinedRecipes();

                    RecipesView = CollectionViewSource.GetDefaultView(recipes);
                    RecipesView.Filter = (object item) => (item as RecipeViewModel).ProductName.ToLower().Contains(ProductName.ToLower());
                });
            }));
        }

        private void LoadRecipesOfSelectedOrder()
        {
            recipes = new List<RecipeViewModel>(selectedOrder.Recipes);
            foreach (var r in recipes)
            {
                ActivateRecipesChecking(r);
                r.IsChecked = true;
            }
        }

        private void LoadUndefinedRecipes()
        {
            IList<int> excludedProductIds = recipes.Select(r => r.ProductId).ToList();
            IList<RecipeDTO> undefinedRecipesDTOs = orderingService.GetUndefinedRecipes(excludedProductIds);

            foreach (var dto in undefinedRecipesDTOs)
            {
                var recipe = new RecipeViewModel(dto, recipes.Count + 1);
                ActivateRecipesChecking(recipe);
                recipes.Add(recipe);
            }
        }

        private void ActivateRecipesChecking(RecipeViewModel recipe)
        {
            recipe.RequiredDataCollectedAction += () =>
            {
                var payload = new RecipesOfOrderCollectedPayload
                {
                    Recipes = recipes.Where(rec => rec.IsChecked && rec.MeasureCount != decimal.Zero).ToList(),
                    IsValid = recipes.Where(rec => rec.IsChecked).Count() == recipes.Where(rec => rec.MeasureCount != decimal.Zero).Count()
                };

                eventAggregator.GetEvent<RecipesOfOrderCollectedEvent>().Publish(payload);
            };
        }
    }
}
