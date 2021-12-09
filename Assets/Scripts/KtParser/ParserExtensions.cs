using System;
using Sprache;

namespace DefaultNamespace.KtParser
{
    public static class Some
    {
        public static Some<T> Of<T>(T value) => new Some<T>(value);
    }

    public static class None
    {
        public static None<T> Of<T>() => new None<T>();
    }

    public static class Parser
    {
        public static Parser<IOption<T>> Zero<T>() => input => Result.Success((IOption<T>) None.Of<T>(), input);
    }
    
    public class Some<T> : IOption<T>
    {
        private readonly T _value;

        public Some(T value)
        {
            _value = value;
        }

        public T GetOrDefault() => _value;
        public T Get() => _value;

        public bool IsEmpty => false;
        public bool IsDefined => true;
    }
    
    public class None<T> : IOption<T>
    {
        public T GetOrDefault() => default;
        public T Get()
        {
            throw new Exception("Trying to get access to None");
        }
        public bool IsEmpty => true;
        public bool IsDefined => false;
    }

    public static class ParserExtensions
    {
        public static T1 Match<T0, T1>(
            this IOption<T0> matched,
            Func<T0, T1> some,
            Func<T1> none
        ) => matched.IsEmpty ? none() : some(matched.Unwrap());

        public static IOption<T1> Map<T0, T1>(
            this IOption<T0> mapped,
            Func<T0, T1> mutator
        ) => mapped.Match(
            some: x => Some.Of(mutator(x)),
            none: () => (IOption<T1>) None.Of<T1>()
        );

        public static bool Let<T0>(
            this IOption<T0> mapped, 
            Action<T0> letAction
        ) => mapped
            .Map(x => { letAction(x); return true; })
            .UnwrapOrDefault(false);

        public static IOption<T1> Select<T0, T1>(
            this IOption<T0> mapped, 
            Func<T0, T1> mutator
        ) => mapped.Map(mutator);

        public static IOption<T1> Bind<T0, T1>(
            this IOption<T0> bound, 
            Func<T0, IOption<T1>> binder
        ) => bound
            .Map(binder)
            .UnwrapOrDefault(new None<T1>());

        public static IOption<TR> SelectMany<T0, T1, TR>(
            this IOption<T0> src,
            Func<T0, IOption<T1>> joinSelector,
            Func<T0, T1, TR> resultSelector
        ) => src
            .Bind(joinSelector)
            .Map(b => resultSelector(src.Unwrap(), b));

        public static IOption<T> Where<T>(
            this IOption<T> source,
            Func<T, bool> predicate
        ) => source.Bind(s => predicate(s) ? new Some<T>(s) : (IOption<T>) new None<T>());
        
        public static T Unwrap<T>(this IOption<T> mapped) => mapped.Get();
        public static T UnwrapOrDefault<T>(
            this IOption<T> mapped, 
            T defaultValue
        ) => mapped.IsDefined
            ? mapped.Get()
            : defaultValue;

        public static Parser<U> Map<T, U>(this Parser<T> mapped, Func<T, U> mapFoo) =>
            mapped.Select(mapFoo);
        
        public static Parser<U> Bind<T, U>(this Parser<T> bound, Func<T, Parser<U>> bindFoo) =>
            bound.Then(bindFoo);

        public static Parser<(T0, T1)> Seq<T0, T1>(this Parser<T0> p0, Parser<T1> p1) =>
            p0.Bind(a => 
            p1.Map( b => 
                (a, b)
            ));

        public static Parser<T> SurroundBy<TL, T, TR>(this Parser<T> p0, Parser<TL> pl, Parser<TR> pr) =>
            pl.Bind(a => 
            p0.Bind( b =>
            pr.Map(c =>
                b      
            )));

        public static Parser<T> Braces<T>(this Parser<T> p, char lBrace, char rBrace) =>
            p.SurroundBy(Parse.Char(lBrace).Token(), Parse.Char(rBrace).Token());
    }
}