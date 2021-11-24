import { useEffect } from "react";
import LoadingComponent from "../../app/layout/LoadingComponent";
import { useAppDispatch, useAppSelector } from "../../app/store/configureStore";
import { fetchProductsAsync, productSelectors } from "./catalogSlice";
import ProductList from "./ProductList";

// interface Props{
//     products: Product[];
//     addProduct: ()=>void;
// }

export default function Catalog(){
    //const [products, setProducts]=useState<Product[]>([]);
    const products=useAppSelector(productSelectors.selectAll);
    const {productsLoaded, status}=useAppSelector(state=>state.catalog);
    const dispatch=useAppDispatch();
    //const [loading, setLoading]=useState(true);

    useEffect(()=> {
      if(!productsLoaded) dispatch(fetchProductsAsync());
      // agent.Catalog.list()
      // .then(products=> setProducts(products))
      // .catch(error=>console.log(error))
      // .finally(()=>setLoading(false))
    },[productsLoaded, dispatch])

    // useEffect(()=>{
    //   fetch('http://localhost:5000/api/products')
    //     .then(response=>response.json())
    //     .then(data=>setProducts(data))
    // }, [])
  
    // function addProduct(){
    //   setProducts(prevState=>[...prevState, 
    //     {
    //       id: prevState.length+101,
    //       name:'product' +(prevState.length+1), 
    //       price:(prevState.length*100)+100,
    //       brand: 'some brand',
    //       description:'some description',
    //       pictureUrl:'https://picsum.photos/200'
    //     }])
    // }

    if (status.includes('pending')) return <LoadingComponent message='Loading products...' />


    return(
       <>
            <ProductList products={products}></ProductList>
            {/* <Button variant='contained' onClick={addProduct}>Add Product</Button> */}
       </>
    )
}