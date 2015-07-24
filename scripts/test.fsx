(*
let x = 1
let y = 2
x + y
|> printfn "%i"

let fizzbuzz n =
    let rec loop acc n =
        if n <= 0 then acc else
        let result =
            match n % 3, n % 5 with
            | 0, 0 -> "FizzBuzz"
            | 0, _ -> "Fizz"
            | _, 0 -> "Buzz"
            | _, _ -> string n
        loop (result::acc) (n - 1)
    loop [] n

fizzbuzz 10
|> List.iter (printfn "%s")
*)

let host : ScriptCs.Contracts.IScriptHost = unbox env.["Host"]
let adder = host.Require<Adder>()
adder.Add(3, 4) |> printfn "%i"
