#r "nuget: FsCheck, 3.0.0-alpha5"

open FsCheck

let revRevIsOrig (xs:list<int>) =
  List.rev xs = xs

Check.Quick revRevIsOrig
