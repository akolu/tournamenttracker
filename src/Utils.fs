module Tournament.Utils

let replace fn item list =
    List.map (fun i -> if fn i then item else i) list

let (>>=) x f = Result.bind f x