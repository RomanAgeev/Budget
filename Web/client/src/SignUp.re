let signUpJson = (~username: string, ~password: string) =>
    Json_encode.(
        object_([
            ("username", string(username)),
            ("password", string(password)),
        ])
    );

let singUpPost = (~username: string, ~password: string) => {
    open Fetch;
    
    let body = signUpJson(~username, ~password)
        |> Json.stringify
        |> BodyInit.make;

    let headers = { "Content-Type": "application/json" }
        |> HeadersInit.make;

    let request = fetchWithInit(
        "http://localhost:3000/signup",
        RequestInit.make(~method_ = Post, ~body, ~headers, ())
    );
    
    Js.Promise.(request
        |> then_(res => Response.ok(res) -> resolve)
    );
};

[@react.component]
let make = () => {
    open ReactEvent.Form;

    let (username, setUsername) = React.useState(() => "");
    let (password, setPassword) = React.useState(() => "");
    let (passwordRepeat, setPasswordRepeat) = React.useState(() => "");
    let (signedUp, setSignedUp) = React.useState(() => false);

    let onUsenameChange = event => {
        let value = target(event)##value;
        setUsername(_ => value);
    };
    let onPasswordChange = event => {
        let value = target(event)##value;
        setPassword(_ => value);
    };
    let onPasswordRepeatChange = event => {
        let value = target(event)##value;
        setPasswordRepeat(_ => value)
    };

    let onSubmit = event => {
        ReactEvent.Mouse.preventDefault(event);

        let _ = Js.Promise.(singUpPost(~username, ~password)
            |> then_(success => {
                setSignedUp(_ => success);
                resolve(());
            })
        );
    };

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
        { ReasonReact.string(signedUp ? "Signed Up" : "Not Yet") }
    </div>
};
