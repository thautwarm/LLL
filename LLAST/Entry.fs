﻿module Entry

open LL.Emit
open LL.Exc
open LL.Helper
open LL.IR
open LL.Infras
open LL.Pass
open LL.ControlFlow
open System.IO

open LL.Lisp
open FastParse

let codegen (title : string) (llvm : llvm) : unit =
    let ctx = context.init
    let type_table = hashtable()
    let emit' = emit <| type_table
    try
    let _, proc = emit' ctx <| visit ctx (fun ctx -> elimIfElse ctx >> elimWhile ctx) llvm
    System.IO.File.WriteAllText(fmt "../ir-snippets/%s.ll" title, proc.to_ir)
    with LLException(exc) ->
        printfn "test %s failed" title
        printfn "%A" exc

[<EntryPoint>]
let main args =
    let codegenWithP name s = (lex >> Parser.parse llvm >> codegen name) s
    let testP p s = (Parser.parse p << lex) <| s
    let testL s = lex <| s |>  printfn "%A"

    0

     