# Functional Programing in c#

**Response&lt;TValue&gt;** is a simple class (monad) that contains value or errors.

Here is a simple usage of return error. Usage for validation methods.

```csharp
    public static Response<int> DivideByZero(int value, int divider)
    {
        if (divider == 0)
        {
            return "Divider value cannot be zero.";
        }

        return value / divider;
    }
```

&nbsp;

Combine methods by using **Then** expression

```csharp

    public static Response ProcessByUserId(int userId)
    {
        return GetUserFromDatabase(userId)
              .Then(SendEMailForUser)
              .Then(MarkUserAsMailNotificationCompleted)
              .Then(SaveUser);
    }

    public static Response<UserInfo> GetUserFromDatabase(int userId)
    {
        throw new NotImplementedException();
    }

    public static Response<UserInfo> SendEMailForUser(UserInfo userInfo)
    {
        throw new NotImplementedException();
    }

    public static Response<UserInfo> MarkUserAsMailNotificationCompleted(UserInfo userInfo)
    {
        throw new NotImplementedException();
    }

    public static Response<UserInfo> SaveUser(UserInfo userInfo)
    {
        throw new NotImplementedException();
    }
    public class UserInfo
    {
    }
```
&nbsp;

Usage of **Pipe**

```csharp

    public static Response ProcessByUserId(int userId)
    {
        return Pipe(userId,
                    GetUserFromDatabase,
                    SendEMailForUser,
                    MarkUserAsMailNotificationCompleted,
                    SaveUser);
    }
```
&nbsp;

Here is my best practices in functional programing
--------------------------------------------------

- Avoid mutable states

- Avoid side effect

- Methods should be pure

- Write code in declerative style

- Seperate processed data and methods

- Write for humans not machines

- Use inline methods for reduce parameter passing problems

- if method is too complex then seperate flow into a pipe method so you can read main code block more clearly. No need to compile codes in your brain.

- Do not pass any monad as parameter to another method.