# Recipes - Prism

Installation instructions:
1. Install MySQL Server with MySQL Workbench;
2. Open MySQL Workbench and fire: `Recipes Sql Scripts/Initialization.sql`. If You want to change database user password then go ahead, but don't forget to change it inside `Recipes Prism.exe.config`. Beware: this file will be encrypted after the first application run so it's very good idea to backup it and restore for changing due to connection failure;
3. Run `Recipes Prism.exe`. All `Recipes - Prism` tables will be created automatically via NHibarnate ORM. 

