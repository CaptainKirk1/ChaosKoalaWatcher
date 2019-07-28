using System.Collections.Generic;

namespace CustomTypes
{
    public static class Monad //monad helpers
    {
        public static Monad<T> ToMonad<T>(this T val) => Monad<T>.Create(val);

        public static Monad<T> Create<T>(T val) => Monad<T>.Create(val);

        public static Monad<T> None<T>() => Monad<T>.None;
    }

    //monad type for wrapping more fundamental type, but carrying info with it, such as none (not instantiated),
    //similar to "nullable", but doing away with handing of null for the type externally
    public class Monad<T>
    {
        private Monad(T value) => Value = value;

        public static Monad<T> Create(T value) => new Monad<T>(value);

        public bool HasValue() => !EqualityComparer<T>.Default.Equals(Value, default(T));

        public static Monad<T> None => new Monad<T>(default(T));

        public T Value { get; private set; }
    }
}
