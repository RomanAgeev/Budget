let i = 0;

[%debugger]

let j = i + 1;

print_int(j);

[@react.component]
let make = () =>
    <h1> { ReasonReact.string("Hello from reason react !") }</h1>;
