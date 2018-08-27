﻿module LLVM.IR
(**
Meta language constructs which could be emitted to LLVM IR.
*)

type location = {
(**
for location resolving in exception reporting.
   *)
    filename: string
    lineno  : int
    colno   : int
}

type 'a undecided = {
(**
for prospective partial processing at AST level.
   *)
    id: int
}

type 'v asoc_list = (string * 'v) list

type ``type`` =
| I     of bit: int
| F     of bit: int
| Arr   of len: int * ``type``
| Vec   of len: int * ``type``
| Agg   of ``type`` list
| Func  of ``type`` list * ``type``
| Ptr   of ``type``
| Alias of name: string
| Label      (** not sure to handle make abstractions for label type*)
| Terminator (** only works when jump appears inner expression.*)
| Void
| PendingTy of ``type`` undecided

type llvm =
(** binary operations including comparisons *)
| Bin         of binary_operation

(** application *)
| App         of llvm * llvm list

(** get local var *)
| Get         of name: string

(** bind local var which create new context*)
| Let         of name: string * value: llvm * body: llvm

(** define function **)
| Defun       of name: string * args: ``type`` asoc_list * ret_ty: ``type`` * body : llvm

| Const       of constant

| DefTy       of name: string * ``type`` list

(** control flow*)
| Switch      of cond: llvm * cases: (llvm * string) list * default': string
| IndrBr      of cond: llvm * labels: string list
| Return      of llvm
| Mark        of name: string (** make label*)
| Branch      of cond: llvm * iftrue: string * iffalse: string
| Jump        of label: string

(** conversions: trunc, fptrunc, ..., bitcast *)
| ZeroExt     of src: llvm * dest: ``type``    // zext; Ext means extending
| CompatCast  of src: llvm * dest: ``type``
| Bitcast     of src: llvm * dest: ``type``

(** data accessing and manipulations *)
(**
including: load, store, alloca, getelementptr
           for aggregate:
            extractvalue
            insertvalue
           for vec:
            extractelement
            insertelement
*)
| Alloca      of ``type`` * data: llvm option
| Load        of name: string
| Store       of name: string * data: llvm
| GEP         of name: string * idx : llvm * offsets: int list
| ExtractElem of subject: llvm * idx: llvm
| InsertElem  of subject: llvm * val': llvm * idx: llvm
| ExtractVal  of subject: llvm * indices: int list
| InsertVal   of subject: llvm * val': llvm * indices: int list

(** others *)
| Suite       of llvm list
| Locate      of location * llvm
| PendingLLVM of llvm undecided
(**
switch, indirectbr, return are all terminators and returns Wildcard.

    *)

(** the name must be in current jump area. **)

and binary_operator =
| Add
| Sub
| Mul
| Div
| Rem
| LSh
| LShr
| AShr
| And
| Or
| XOr
// comparator
| Eq
| Gt
| Ge
| Lt
| Le
| Ne
| PendingBinOp of binary_operator undecided

and binary_operation = binary_operator * llvm * llvm

and constant =
| ID    of bit: int * value: int64
| FD    of bit: int * value: float
| ArrD  of constant list
| VecD  of constant list
| AggD  of constant list
| Undef of ``type``
| PendingConst of constant undecided