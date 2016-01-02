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

module FSharpUnionSerialization =

    type Primitive =
       | Bool of bool
       | Int of int
       | String of string
       | Float of float

    [<Test>]
    let ``test serialize primitives in a union case``() =
        [ (Bool false, BsonDocument([ BsonElement("_t", BsonString "Bool")
                                      BsonElement("Item", BsonBoolean false) ]))
          (Int 0, BsonDocument([ BsonElement("_t", BsonString "Int")
                                 BsonElement("Item", BsonInt32 0) ]))
          (String "0.0", BsonDocument([ BsonElement("_t", BsonString "String")
                                        BsonElement("Item", BsonString "0.0") ]))
          (Float 0.0, BsonDocument([ BsonElement("_t", BsonString "Float")
                                     BsonElement("Item", BsonDouble 0.0) ])) ]
        |> List.iter (fun (value, expected) ->
            let result = <@ serialize value @>
            test <@ %result = expected @>)

    [<Test>]
    let ``test deserialize primitives in a union case``() =
        [ (Bool true, BsonDocument([ BsonElement("_t", BsonString "Bool")
                                     BsonElement("Item", BsonBoolean true) ]))
          (Int 1, BsonDocument([ BsonElement("_t", BsonString "Int")
                                 BsonElement("Item", BsonInt32 1) ]))
          (String "1.0", BsonDocument([ BsonElement("_t", BsonString "String")
                                        BsonElement("Item", BsonString "1.0") ]))
          (Float 1.0, BsonDocument([ BsonElement("_t", BsonString "Float")
                                     BsonElement("Item", BsonDouble 1.0) ])) ]
        |> List.iter (fun (expected, doc) ->
            let result = <@ deserialize doc typeof<Primitive> @>
            test <@ %result = expected @>)

    module Arity =

        type Number =
           | Zero
           | One of int
           | Two of int * int
           | Three of int * int * int

        [<Test>]
        let ``test serialize a union case with arity 0``() =
            let value = Zero

            let result = <@ serialize value @>
            let expected = BsonDocument([ BsonElement("_t", BsonString "Zero") ])

            test <@ %result = expected @>

        [<Test>]
        let ``test deserialize a union case with arity 0``() =
            let doc = BsonDocument([ BsonElement("_t", BsonString "Zero") ])

            let result = <@ deserialize doc typeof<Number> @>
            let expected = Zero

            test <@ %result = expected @>

        [<Test>]
        let ``test serialize a union case with arity 1``() =
            let value = One 1

            let result = <@ serialize value @>
            let expected = BsonDocument([ BsonElement("_t", BsonString "One")
                                          BsonElement("Item", BsonInt32 1) ])

            test <@ %result = expected @>

        [<Test>]
        let ``test deserialize a union case with arity 1``() =
            let doc = BsonDocument([ BsonElement("_t", BsonString "One")
                                     BsonElement("Item", BsonInt32 1) ])

            let result = <@ deserialize doc typeof<Number> @>
            let expected = One 1

            test <@ %result = expected @>

        [<Test>]
        let ``test serialize a union case with arity 2``() =
            let value = Two (1, 2)

            let result = <@ serialize value @>
            let expected = BsonDocument([ BsonElement("_t", BsonString "Two")
                                          BsonElement("Item1", BsonInt32 1)
                                          BsonElement("Item2", BsonInt32 2) ])

            test <@ %result = expected @>

        [<Test>]
        let ``test deserialize a union case with arity 2``() =
            let doc = BsonDocument([ BsonElement("_t", BsonString "Two")
                                     BsonElement("Item1", BsonInt32 1)
                                     BsonElement("Item2", BsonInt32 2) ])

            let result = <@ deserialize doc typeof<Number> @>
            let expected = Two (1, 2)

            test <@ %result = expected @>

        [<Test>]
        let ``test serialize a union case with arity 3``() =
            let value = Three (1, 2, 3)

            let result = <@ serialize value @>
            let expected = BsonDocument([ BsonElement("_t", BsonString "Three")
                                          BsonElement("Item1", BsonInt32 1)
                                          BsonElement("Item2", BsonInt32 2)
                                          BsonElement("Item3", BsonInt32 3) ])

            test <@ %result = expected @>

        [<Test>]
        let ``test deserialize a union case with arity 3``() =
            let doc = BsonDocument([ BsonElement("_t", BsonString "Three")
                                     BsonElement("Item1", BsonInt32 1)
                                     BsonElement("Item2", BsonInt32 2)
                                     BsonElement("Item3", BsonInt32 3) ])

            let result = <@ deserialize doc typeof<Number> @>
            let expected = Three (1, 2, 3)

            test <@ %result = expected @>

    module NullUnion =

        type Letter =
           | A
           | B
           | C
           | D

        [<Test>]
        let ``test serialize a null union case``() =
            [ (A, BsonDocument([ BsonElement("_t", BsonString "A") ]))
              (B, BsonDocument([ BsonElement("_t", BsonString "B") ]))
              (C, BsonDocument([ BsonElement("_t", BsonString "C") ]))
              (D, BsonDocument([ BsonElement("_t", BsonString "D") ])) ]
            |> List.iter (fun (value, expected) ->
                let result = <@ serialize value @>
                test <@ %result = expected @>)

        [<Test>]
        let ``test deserialize a null union case``() =
            [ (A, BsonDocument([ BsonElement("_t", BsonString "A") ]))
              (B, BsonDocument([ BsonElement("_t", BsonString "B") ]))
              (C, BsonDocument([ BsonElement("_t", BsonString "C") ]))
              (D, BsonDocument([ BsonElement("_t", BsonString "D") ])) ]
            |> List.iter (fun (expected, doc) ->
                let result = <@ deserialize doc typeof<Letter> @>
                test <@ %result = expected @>)

    module Singleton =

        [<RequireQualifiedAccess>]
        type Only0 = | Case

        [<RequireQualifiedAccess>]
        type Only1 = | Case of int

        [<RequireQualifiedAccess>]
        type Only2 = | Case of int * int

        [<RequireQualifiedAccess>]
        type Only3 = | Case of int * int * int

        [<Test>]
        let ``test serialize a singleton union type with arity 0``() =
            let value = Only0.Case

            let result = <@ serialize value @>
            let expected = BsonDocument([ BsonElement("_t", BsonString "Case") ])

            test <@ %result = expected @>

        [<Test>]
        let ``test deserialize a singleton union type with arity 0``() =
            let doc = BsonDocument([ BsonElement("_t", BsonString "Case") ])

            let result = <@ deserialize doc typeof<Only0> @>
            let expected = Only0.Case

            test <@ %result = expected @>

        [<Test>]
        let ``test serialize a singleton union type with arity 1``() =
            let value = Only1.Case 1

            let result = <@ serialize value @>
            let expected = BsonDocument([ BsonElement("_t", BsonString "Case")
                                          BsonElement("Item", BsonInt32 1) ])

            test <@ %result = expected @>

        [<Test>]
        let ``test deserialize a singleton union type with arity 1``() =
            let doc = BsonDocument([ BsonElement("_t", BsonString "Case")
                                     BsonElement("Item", BsonInt32 1) ])

            let result = <@ deserialize doc typeof<Only1> @>
            let expected = Only1.Case 1

            test <@ %result = expected @>

        [<Test>]
        let ``test serialize a singleton union type with arity 2``() =
            let value = Only2.Case (1, 2)

            let result = <@ serialize value @>
            let expected = BsonDocument([ BsonElement("_t", BsonString "Case")
                                          BsonElement("Item1", BsonInt32 1)
                                          BsonElement("Item2", BsonInt32 2) ])

            test <@ %result = expected @>

        [<Test>]
        let ``test deserialize a singleton union type with arity 2``() =
            let doc = BsonDocument([ BsonElement("_t", BsonString "Case")
                                     BsonElement("Item1", BsonInt32 1)
                                     BsonElement("Item2", BsonInt32 2) ])

            let result = <@ deserialize doc typeof<Only2> @>
            let expected = Only2.Case (1, 2)

            test <@ %result = expected @>

        [<Test>]
        let ``test serialize a singleton union type with arity 3``() =
            let value = Only3.Case (1, 2, 3)

            let result = <@ serialize value @>
            let expected = BsonDocument([ BsonElement("_t", BsonString "Case")
                                          BsonElement("Item1", BsonInt32 1)
                                          BsonElement("Item2", BsonInt32 2)
                                          BsonElement("Item3", BsonInt32 3) ])

            test <@ %result = expected @>

        [<Test>]
        let ``test deserialize a singleton union type with arity 3``() =
            let doc = BsonDocument([ BsonElement("_t", BsonString "Case")
                                     BsonElement("Item1", BsonInt32 1)
                                     BsonElement("Item2", BsonInt32 2)
                                     BsonElement("Item3", BsonInt32 3) ])

            let result = <@ deserialize doc typeof<Only3> @>
            let expected = Only3.Case (1, 2, 3)

            test <@ %result = expected @>
