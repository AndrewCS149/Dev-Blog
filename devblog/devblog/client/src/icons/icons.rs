use yew::prelude::*;

#[derive(Properties, PartialEq)]
pub struct Props {
    pub vote_onclick: Callback<String>,
}

#[function_component(UpVoteIcon)]
pub fn upvote(props: &Props) -> Html {
    let on_click = {
        let onclick = props.vote_onclick.clone();
        Callback::from(move |_| onclick.emit("upvote".to_string()))
    };
    html! {<svg onclick={on_click} class={"up-vote"} xmlns="http://www.w3.org/2000/svg" viewBox="0 -960 960 960"><path d="M480-260q68 0 123.5-38.5T684-400H276q25 63 80.5 101.5T480-260ZM312-520l44-42 42 42 42-42-84-86-86 86 42 42Zm250 0 42-42 44 42 42-42-86-86-84 86 42 42ZM480-80q-83 0-156-31.5T197-197q-54-54-85.5-127T80-480q0-83 31.5-156T197-763q54-54 127-85.5T480-880q83 0 156 31.5T763-763q54 54 85.5 127T880-480q0 83-31.5 156T763-197q-54 54-127 85.5T480-80Zm0-400Zm0 320q134 0 227-93t93-227q0-134-93-227t-227-93q-134 0-227 93t-93 227q0 134 93 227t227 93Z"/></svg>}
}

#[function_component(DownVoteIcon)]
pub fn downvote(props: &Props) -> Html {
    let on_click = {
        let onclick = props.vote_onclick.clone();
        Callback::from(move |_| onclick.emit("downvote".to_string()))
    };
    html! {<svg onclick={on_click} class={"down-vote"} xmlns="http://www.w3.org/2000/svg" viewBox="0 -960 960 960"><path d="M480-420q-68 0-123.5 38.5T276-280h408q-25-63-80.5-101.5T480-420Zm-168-60 44-42 42 42 42-42-42-42 42-44-42-42-42 42-44-42-42 42 42 44-42 42 42 42Zm250 0 42-42 44 42 42-42-42-42 42-44-42-42-44 42-42-42-42 42 42 44-42 42 42 42ZM480-80q-83 0-156-31.5T197-197q-54-54-85.5-127T80-480q0-83 31.5-156T197-763q54-54 127-85.5T480-880q83 0 156 31.5T763-763q54 54 85.5 127T880-480q0 83-31.5 156T763-197q-54 54-127 85.5T480-80Zm0-400Zm0 320q134 0 227-93t93-227q0-134-93-227t-227-93q-134 0-227 93t-93 227q0 134 93 227t227 93Z"/></svg>}
}

#[function_component(TrashIcon)]
pub fn trash() -> Html {
    html! {<svg viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path d="M8 1.5V2.5H3C2.44772 2.5 2 2.94772 2 3.5V4.5C2 5.05228 2.44772 5.5 3 5.5H21C21.5523 5.5 22 5.05228 22 4.5V3.5C22 2.94772 21.5523 2.5 21 2.5H16V1.5C16 0.947715 15.5523 0.5 15 0.5H9C8.44772 0.5 8 0.947715 8 1.5Z"/><path d="M3.9231 7.5H20.0767L19.1344 20.2216C19.0183 21.7882 17.7135 23 16.1426 23H7.85724C6.28636 23 4.98148 21.7882 4.86544 20.2216L3.9231 7.5Z"/></svg>}
}

#[function_component(EditIcon)]
pub fn edit() -> Html {
    html! {<svg viewBox="0 -0.5 21 21" version="1.1" xmlns="http://www.w3.org/2000/svg"><g  stroke-width="1"><g id="Dribbble-Light-Preview" transform="translate(-59.000000, -400.000000)"><g transform="translate(56.000000, 160.000000)"><path d="M3,260 L24,260 L24,258.010742 L3,258.010742 L3,260 Z M13.3341,254.032226 L9.3,254.032226 L9.3,249.950269 L19.63095,240 L24,244.115775 L13.3341,254.032226 Z"></path></g></g></g></svg>}
}

#[function_component(ArrowRightIcon)]
pub fn arrow_right() -> Html {
    html! {<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 -960 960 960"><path d="M504-480 320-664l56-56 240 240-240 240-56-56 184-184Z"/></svg>}
}

#[function_component(ArrowLeftIcon)]
pub fn arrow_left() -> Html {
    html! {<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 -960 960 960"><path d="M560-240 320-480l240-240 56 56-184 184 184 184-56 56Z"/></svg>}
}

#[function_component(MenuIcon)]
pub fn menu() -> Html {
    html! {<svg class="nav-icon" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path d="M4 6H20M4 12H20M4 18H20" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg>}
}

#[function_component(BellIcon)]
pub fn bell() -> Html {
    html! {<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512"><path d="M224 0c-17.7 0-32 14.3-32 32V51.2C119 66 64 130.6 64 208v18.8c0 47-17.3 92.4-48.5 127.6l-7.4 8.3c-8.4 9.4-10.4 22.9-5.3 34.4S19.4 416 32 416H416c12.6 0 24-7.4 29.2-18.9s3.1-25-5.3-34.4l-7.4-8.3C401.3 319.2 384 273.9 384 226.8V208c0-77.4-55-142-128-156.8V32c0-17.7-14.3-32-32-32zm45.3 493.3c12-12 18.7-28.3 18.7-45.3H224 160c0 17 6.7 33.3 18.7 45.3s28.3 18.7 45.3 18.7s33.3-6.7 45.3-18.7z"/></svg>}
}
