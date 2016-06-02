namespace Shared.Frameworks.DataAccess.CodeGen
{
    public class ValueObject<T>
    {
        
    }

    /// <summary>
    /// Represents a script of some sort
    /// </summary>
    public sealed class Script : ValueObject<Script>
    {
        /// <summary>
        /// Implicitly converts text to a <see cref="Script"/>
        /// </summary>
        /// <param name="x">The script body</param>
        public static implicit operator Script(string x) => new Script(x);

        /// <summary>
        /// Implicitly converts <see cref="Script"/> to text
        /// </summary>
        /// <param name="x">The script body</param>
        public static implicit operator string(Script x) => x.Body;

        /// <summary>
        /// The script body
        /// </summary>
        public readonly string Body;

        public Script(string Body)
        {
            this.Body = Body;
        }
    }

    
}
