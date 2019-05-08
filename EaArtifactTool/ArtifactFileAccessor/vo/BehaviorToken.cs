using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArtifactFileAccessor.vo
{
    public class BehaviorToken
    {
        public TokenType tokenType;
        public string token;

        public BehaviorToken NextToken;



    }

    public enum TokenType
    {
        TOKEN_ELEMENT_NAME, TOKEN_ATTRIBUTE_NAME, TOKEN_METHOD_NAME,
        TOKEN_INSTANCE_LABEL, TOKEN_INSTANCE_TYPE,
        TOKEN_PARAMETER_TYPE_NAME, TOKEN_PARAMETER_NAME,
        TOKEN_RETURN, TOKEN_CONTINUE, TOKEN_BREAK, 
        TOKEN_DECLARE, TOKEN_DECLARE_LABEL, TOKEN_FOREACH, TOKEN_COLLECTION_NAME,
        TOKEN_DOT, TOKEN_COMMA, TOKEN_EQUAL, TOKEN_BRACKET_BEGIN, TOKEN_BRACKET_END,
        TOKEN_PARENTHESIS_BEGIN, TOKEN_PARENTHESIS_END,
        TOKEN_SEMICOLON,

        // ざっくり識別子が入っている式（トークナイズが不完全な状態にある）
        TOKEN_EXPR_IDENTIFIER,
        // ざっくり条件式が入っている式
        TOKEN_EXPR_CONDITIONAL,
        // ざっくりメソッド呼び出しが入っている式
        // （内部的に戻り値の取得も含むが、代入式の左辺値にはなれない）
        TOKEN_EXPR_INVOKE_METHOD,
    }

}
