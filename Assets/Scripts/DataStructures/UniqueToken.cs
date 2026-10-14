using LOI.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace LOI.DataStructures
{
    public class UniqueToken : IEquatable<UniqueToken>
    {
        public string Token { get; }
        public char Separator { get; }
        public UniqueToken ChildToken { get; }
        public UniqueToken LastToken { get; private set; }

        public UniqueToken(string key, char separator = '.') 
        {
            string[] tokens = key.Split(separator, 2);
            this.Token = tokens[0];
            this.Separator = separator;
            if(tokens.Length > 1)
            {
                this.ChildToken = new UniqueToken(tokens[1], separator);
                this.LastToken = this.ChildToken.LastToken;
            }
            else
            {
                this.ChildToken = null;
                this.LastToken = this;
            }
        }

        public UniqueToken(string token, char separator, UniqueToken childToken)
        {
            this.Token = token;
            this.Separator = separator;
            this.ChildToken = childToken;
            this.LastToken = childToken?.LastToken ?? this;
        }

        public static implicit operator string(UniqueToken token) => token?.ToString();
        public static implicit operator UniqueToken(string token) => new UniqueToken(token, '.');
        public static UniqueToken operator +(string key, UniqueToken token) => new UniqueToken(key, token.Separator, token.ChildToken);
        public static UniqueToken operator +(UniqueToken token, string key) => new UniqueToken(token, token.LastToken.Separator, key);
        public static UniqueToken operator +(UniqueToken token1, UniqueToken token2) => new UniqueToken(token1, token1.LastToken.Separator, token2);

        public static bool operator ==(UniqueToken left, UniqueToken right) => object.Equals(left, right);
        public static bool operator !=(UniqueToken left, UniqueToken right) => !object.Equals(left, right);

        public override string ToString() => this.ChildToken == null ? this.Token : string.Concat(this.Token, this.Separator, this.ChildToken.ToString());

        public override bool Equals(object obj) => Equals(obj as UniqueToken);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + Separator.GetHashCode();
                hash = hash * 31 + StringComparer.Ordinal.GetHashCode(ToString());
                return hash;
            }
        }

        public bool Equals(UniqueToken other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(other, null)) return false;
            return this.Separator == other.Separator && string.Equals(ToString(), other.ToString(), StringComparison.Ordinal);
        }
    }
}