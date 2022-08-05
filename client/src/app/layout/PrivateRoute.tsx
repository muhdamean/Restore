import { Navigate, Outlet, OutletProps, useLocation } from "react-router-dom";
import { toast } from "react-toastify";
import { useAppSelector } from "../store/configureStore";


// interface Props {
//     location:string;
// }
// interface Props extends RouteProps{ 
//     component: ComponentType<RouteComponentProps<any>> | ComponentType<any>
// }
interface Props extends OutletProps{ 
    roles?: string[];
}

export default function PrivateRoute({roles, ...options}:Props){
    const location = useLocation();
    const {user}=useAppSelector(state=>state.account);

    if(!user) return <Navigate to="/login" state={{ from: location }} />

    if(roles && !roles?.some(r=>user.roles?.includes(r))) {
        toast.error('Not authorised to access this area');
        return <Navigate to="/catalog" />
    }

    return (
       <Outlet {...options} />
    )

    // return (
    //     user ? <Outlet {...options} /> : <Navigate to="/login" state={{ from: location }} />
    // )
}


// export default function PrivateRoute({component: Component, ...rest}: Props){
//     const {user}=useAppSelector(state=>state.account);
//     return (
//         <Route 
//             {...rest}
//             render={props=>
//                 user ?(
//                     <Component {...props} />
//                 ): (
//                     <Navigate to="/login" state={props.location}
//                     />
//                 )
//             }
//         />
//     )
// }