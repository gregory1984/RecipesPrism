CREATE SCHEMA `Recipes`;
CREATE USER 'RecipesUser' IDENTIFIED BY 'RecipesPass';
GRANT ALL PRIVILEGES ON Recipes.* TO 'RecipesUser';