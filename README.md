# Сервис CRUD методотов сущности User
## Поддерживаемые методы:
1. **CREATE** *POST /users/create* - создать сущность
2. **UPDATE** *PUT /users/update-name-gender-birthday/{guid}* - обновить поля имя, пол и дата рождения сущности
3. **UPDATE** *PUT /users/update-password/{guid}* - обновить пароль сущности
4. **UPDATE** *PUT /users/update-login/{guid}* - обновить логин сущности
5. **READ** *GET /users/active* - получить список активных сущностей
6. **READ** *GET /users/by-login/{userLogin}* - получить сущность по её логину
7. **READ** *GET /users/me* - получить сущность самого себя
8. **READ** *GET /users/older-that/{years}* - получить список сущностей страрше определённого возраста
9. **DELETE** *DELETE /users/delete/{guid}* - удалить сущность
10. **UPDATE** *PATCH /users/recovery/{guid}* - восстановить сущность
## Данные:
* В header'е каждого запроса должен содержаться логин (login) и пароль (password)
* Тело запроса либо пустое, либо содержит json
* Ответ на запрос всегда представляет собой json
* Изначально создаётся администратор с паролем и логином: Admin
