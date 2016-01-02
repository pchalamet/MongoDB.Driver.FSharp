(* Copyright (c) 2015 MongoDB, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *)

namespace FSharp.MongoDB.Bson.Tests.Serialization

open MongoDB.Bson

open NUnit.Framework
open Swensen.Unquote

module FSharpRecordSerialization =

    type Primitive =
        { Bool : bool
          Int : int
          String : string
          Float : float }

    [<Test>]
    let ``test serialize primitives in a record type``() =
        let value = { Bool = false
                      Int = 0
                      String = "0.0"
                      Float = 0.0 }

        let result = <@ serialize value @>
        let expected = BsonDocument([ BsonElement("Bool", BsonBoolean false)
                                      BsonElement("Int", BsonInt32 0)
                                      BsonElement("String", BsonString "0.0")
                                      BsonElement("Float", BsonDouble 0.0) ])

        test <@ %result = expected @>

    [<Test>]
    let ``test deserialize primitives in a record type``() =
        let doc = BsonDocument([ BsonElement("Bool", BsonBoolean true)
                                 BsonElement("Int", BsonInt32 1)
                                 BsonElement("String", BsonString "1.0")
                                 BsonElement("Float", BsonDouble 1.0) ])

        let result = <@ deserialize doc typeof<Primitive> @>
        let expected = { Bool = true
                         Int = 1
                         String = "1.0"
                         Float = 1.0 }

        test <@ %result = expected @>
