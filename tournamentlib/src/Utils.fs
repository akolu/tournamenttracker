module Tournament.Utils

open System

let replace fn item list =
    List.map (fun i -> if fn i then item else i) list

let (>>=) x f = Result.bind f x

let unwrap res =
    match res with
    | Ok res -> res
    | Error err -> raise (Exception(err.ToString()))
