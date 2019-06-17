let paylodSignUp = (~username: string, ~password: string) => {
    open Js;

    let payload = Dict.empty();
        Dict.set(payload, "username", Json.string(username));
        Dict.set(payload, "password", Json.string(password));
        Json.object_(payload);
};

[@react.component]
let make = () => {
    open ReactEvent.Form;

    let (username, setUsername) = React.useState(() => "");
    let (password, setPassword) = React.useState(() => "");
    let (passwordRepeat, setPasswordRepeat) = React.useState(() => "");

    let onUsenameChange = event => target(event)##value -> setUsername;
    let onPasswordChange = event => target(event)##value -> setPassword;
    let onPasswordRepeatChange = event => target(event)##value -> setPasswordRepeat;

    let onSubmit = event => {
        open Fetch;
        open Js;
        
        ReactEvent.Mouse.preventDefault(event);

        let body = paylodSignUp(~username, ~password)
            |> Json.stringify
            |> BodyInit.make;

        let headers = { "Content-Type": "application/json" }
            |> HeadersInit.make;

        let request = fetchWithInit(
            "http://localhost:3000/signup",
            RequestInit.make(~method_ = Post, ~body, ~headers, ())
        );
        
        let _ = Promise.(request
            |> then_(res => Response.ok(res) -> resolve)
        );
    };

    // TODO: remove
    React.useEffect(() => {
        open Webapi.Dom;

        document
            |> Document.getElementsByTagName("title")
            |> HtmlCollection.item(0)
            |> item => switch (item) {
                | Some(title) => Element.setInnerText(title, username)
                | None => ()
            };
            None;
    });

    <div className="row">
        <form className="col s12">
            <div className="row">
                <div className="input-field col s12">
                    <input id="username" type_="text" value={ username } onChange={ onUsenameChange } />
                    <label htmlFor="username">
                        { ReasonReact.string("Username") }
                    </label>
                </div>
            </div>
            <div className="row">
                <div className="input-field col s12">
                    <input id="password" type_="password" value={ password } onChange={ onPasswordChange } />
                    <label htmlFor="password">
                        { ReasonReact.string("Password") }
                    </label>
                </div>
            </div>
            <div className="row">
                <div className="input-field col s12">
                    <input id="password_repeat" type_="password" value={ passwordRepeat } onChange={ onPasswordRepeatChange }/>
                    <label htmlFor="password_repeat">
                        { ReasonReact.string("Repeat Password") }
                    </label>
                </div>
            </div>
            <div className="row">
                <button className="btn waves-effect waves-light" type_="submit" onClick={ onSubmit }>
                    { ReasonReact.string("Submit") }
                </button>
            </div>
        </form>
        { ReasonReact.string(username) }
        <br/>
        { ReasonReact.string(password) }
        <br/>
        { ReasonReact.string(passwordRepeat) }
    </div>;
};
